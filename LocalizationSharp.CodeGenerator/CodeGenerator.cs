using System;
using LocalizationSharp.Core;

namespace LocalizationSharp.CodeGenerator
{
    public static class CodeGenerator
    {
        public static string Generate(CodeGenerateOptions options = null, LocalizationManager manager = null)
        {
            if (options == null)
                options = new CodeGenerateOptions();

            if (manager == null)
                manager = LocalizationManager.Instance;

            if (manager == null)
                throw new InvalidOperationException("LocalizationManagerが初期化されていません。");

            ILocalizeFile file = manager.GetFile();

            return string.Empty;
        }

        public class CodeGenerateOptions
        {
            /// <summary>
            /// 生成されるコードのネームスペース
            /// </summary>
            public string Namespace { get; set; } = "LocalizationSharp.Generated";

            /// <summary>
            /// 生成されるコードの型名
            /// </summary>
            public string ClassName { get; set; } = "GeneratedClass";

            /// <summary>
            /// 生成されるコードに追加の 'using' を挿入します。 
            /// </summary>
            public string[] AdditionalUsing { get; set; } = new string[0];

            /// <summary>
            /// 生成されるコードの型へアクセスする方法
            /// </summary>
            public TypeAccessMode TypeAccessMode { get; set; }

            /// <summary>
            /// 生成されるコードのメンバーを生成する方法
            /// </summary>
            public GenerateMemberMode GenerateMemberMode { get; set; }
        }

        public enum TypeAccessMode
        {
            Default,
            QuickAccess,
            Dynamic
        }

        public enum GenerateMemberMode
        {
            ExtensionMethod,
            Property
        }
    }
}