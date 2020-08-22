using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp;

namespace ICsi.Roslyn
{
    internal sealed class ScriptEngineOptions
    {
        public LanguageVersion Version { get; }
        public int WarningLevel { get; }
        public bool AllowUnsafe { get; }
        public string[] Imports { get; }
        public string[] References { get; }

        internal ScriptEngineOptions(LanguageVersion version,
                                     int warningLevel,
                                     bool allowUnsafe,
                                     string[] imports,
                                     string[] references)
        {
            Version = version;
            WarningLevel = warningLevel;
            AllowUnsafe = allowUnsafe;
            Imports = imports;
            References = references;
        }

        public static ScriptEngineOptions Default
            => new ScriptEngineOptions(version: LanguageVersion.CSharp9,
                                       warningLevel: 1,
                                       allowUnsafe: false,
                                       new string[] {},
                                       new string[]
                                       {
                                           typeof(object).Assembly.Location,
                                           typeof(Console).Assembly.Location,
                                           typeof(File).Assembly.Location,
                                           typeof(IEnumerable).Assembly.Location,
                                           typeof(IEnumerable<>).Assembly.Location,
                                           typeof(ImmutableArray<>).Assembly.Location
                                       });

        public ScriptEngineOptions WithVersion(LanguageVersion version)
            => new ScriptEngineOptions(version,
                                       WarningLevel,
                                       AllowUnsafe,
                                       Imports,
                                       References);
        
        public ScriptEngineOptions WithWarningLevel(int warningLevel)
            => new ScriptEngineOptions(Version,
                                       warningLevel,
                                       AllowUnsafe,
                                       Imports,
                                       References);
        
        public ScriptEngineOptions WithAllowUnsafe(bool allowUnsafe)
            => new ScriptEngineOptions(Version, 
                                       WarningLevel,
                                       allowUnsafe,
                                       Imports,
                                       References);
        
        public void AddImports(string[] imports)
            => Imports.ToList().AddRange(imports);

        public void AddReferences(string[] references)
            => References.ToList().AddRange(references);
    }
}