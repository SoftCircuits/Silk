// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace SilkPlatforms
{
    internal abstract class Platform
    {
        public readonly List<FunctionInfo> Functions;
        public readonly List<VariableInfo> Variables;

        public RunForm? Form { get; init; }
        public string Description { get; init; }

        public Platform(SilkPlatform platform)
        {
            Description = RunProgram.GetPlatformDescription(platform);
            if (RequiresForm)
                Form = new RunForm(this);

            Functions = [];
            Variables = [];
        }

        protected void AddFunction(FunctionInfo info) => Functions.Add(info);
        protected void AddVariable(VariableInfo info) => Variables.Add(info);

        public virtual bool RequiresForm => true;

        public virtual void Begin() { }

        public virtual void End() { }

        public virtual void CleanUp() { }
    }
}
