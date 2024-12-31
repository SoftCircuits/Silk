using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Forms;

namespace EmailBlaster
{
    public partial class DocumentManager : Component
    {
        // Public events
        public event EventHandler<DocumentEventArgs>? NewFile;
        public event EventHandler<DocumentEventArgs>? ReadFile;
        public event EventHandler<DocumentEventArgs>? WriteFile;
        public event EventHandler<DocumentEventArgs>? FileChanged;

        /// <summary>
        /// The name of the current document.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? FileName { get; set; }

        /// <summary>
        /// True if the current document has unsaved changes.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsModified { get; set; }

        /// <summary>
        /// The default extension given to documents.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? DefaultExt { get; set; }

        /// <summary>
        /// The common dialog filter for document files.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? Filter { get; set; }

        /// <summary>
        /// Initial working directory.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? InitialDirectory { get; set; }

        public DocumentManager()
        {
            InitializeComponent();
            Initialize();
        }

        public DocumentManager(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            Initialize();
        }

        /// <summary>
        /// Indicates if a filename has been defined for the current document.
        /// </summary>
        [MemberNotNullWhen(true, nameof(FileName))]
        public bool HasFileName => !string.IsNullOrEmpty(FileName);

        /// <summary>
        /// Returns the title of the current file (filename without path) or
        /// "Untitled" if the current file has no name
        /// </summary>
        public string FileTitle => HasFileName ? Path.GetFileName(FileName) : "Untitled";

        protected void Initialize()
        {
            FileName = null;
            IsModified = false;
            DefaultExt = "dat";
            Filter = "All Files (*.*)|*.*";
            InitialDirectory = "";
        }

        /// <summary>
        /// Resets document data and raises the NewFile event.
        /// </summary>
        public bool New()
        {
            if (PromptSaveIfModified())
                return OnNewFile();
            return false;
        }

        /// <summary>
        /// Lets the user browse for a document file and raises the OpenFile event.
        /// </summary>
        public bool Open()
        {
            if (PromptSaveIfModified())
            {
                openFileDialog1.DefaultExt = DefaultExt;
                openFileDialog1.Filter = Filter;
                openFileDialog1.InitialDirectory = InitialDirectory;
                openFileDialog1.FileName = String.Empty;
                openFileDialog1.CheckFileExists = true;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    return OnLoadFile(openFileDialog1.FileName);
            }
            return false;
        }

        /// <summary>
        /// Opens the specified filename. Does not prompt to save current file
        /// </summary>
        /// <param name="path">Full path of file to be opened.</param>
        public bool Open(string path) => OnLoadFile(path);

        /// <summary>
        /// Saves the current document to file. Prompts for a name if the current
        /// document is unnamed.
        /// </summary>
        public bool Save() => (HasFileName) ? OnSaveFile(FileName) : SaveAs();

        /// <summary>
        /// Saves the current file with a new name.
        /// </summary>
        public bool SaveAs()
        {
            saveFileDialog1.DefaultExt = DefaultExt;
            saveFileDialog1.Filter = Filter;
            saveFileDialog1.InitialDirectory = InitialDirectory;
            saveFileDialog1.FileName = FileName;
            saveFileDialog1.OverwritePrompt = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                return OnSaveFile(saveFileDialog1.FileName!);
            return false;
        }

        /// <summary>
        /// Prompts to save the current document if it has been modified.
        /// </summary>
        /// <returns>True if the file was not modified, the user did not want to save the changes, or
        /// if the file was successfully saved. Otherwise, returns false.</returns>
        public bool PromptSaveIfModified()
        {
            if (IsModified)
            {
                DialogResult result = MessageBox.Show("File has been modified. Save changes?", "Save Changes",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    return Save();
                return result != DialogResult.Cancel;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool OnNewFile()
        {
            try
            {
                DocumentEventArgs args = new() { FileName = string.Empty };
                NewFile?.Invoke(this, args);
                FileName = null;
                IsModified = false;
                FileChanged?.Invoke(this, args);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating new file : {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected bool OnLoadFile(string path)
        {
            try
            {
                DocumentEventArgs args = new() { FileName = path };
                ReadFile?.Invoke(this, args);
                FileName = path;
                IsModified = false;
                FileChanged?.Invoke(this, args);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading \"{path}\" : {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected bool OnSaveFile(string path)
        {
            try
            {
                DocumentEventArgs args = new() { FileName = path };
                WriteFile?.Invoke(this, args);
                FileName = path;
                IsModified = false;
                FileChanged?.Invoke(this, args);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving \"{path}\" : {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
    }

    public class DocumentEventArgs : EventArgs
    {
        public string FileName { get; set; } = string.Empty;
    }
}
