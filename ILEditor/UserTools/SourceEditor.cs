﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ILEditor.Classes;
using FastColoredTextBoxNS;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using ILEditor.Classes.LanguageTools;
using System.Threading;

namespace ILEditor.UserTools
{
    public enum ILELanguage
    {
        None,
        CPP,
        RPG
    }

    public partial class SourceEditor : UserControl
    {
        public FastColoredTextBox Editor = null;
        private ILELanguage Language;

        public SourceEditor(String LocalFile, ILELanguage Language = ILELanguage.None)
        {
            InitializeComponent();

            //https://www.codeproject.com/Articles/161871/Fast-Colored-TextBox-for-syntax-highlighting

            this.Language = Language;

            Editor = new FastColoredTextBox();
            Editor.Dock = DockStyle.Fill;

            switch (Language) {
                case ILELanguage.RPG:
                    Editor.TextChanged += SetRPG;
                    break;
                case ILELanguage.CPP:
                    Editor.TextChanged += SetCPP;
                    break;
            }

            Editor.Text = File.ReadAllText(LocalFile);
            
            OnSaveLoad();

            this.Controls.Add(Editor);
        }

        public void OnSaveLoad()
        {
            new Thread((ThreadStart)delegate
            {
                switch (Language)
                {
                    case ILELanguage.RPG:
                        break;
                }
            }).Start();
        }

        #region Styles
        private static readonly Style BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Regular);
        private static readonly Style GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        private static readonly Style MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        private static readonly Style RedStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        private static readonly Style BoldStyle = new TextStyle(Brushes.Black, null, FontStyle.Bold);
        private static readonly Style BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        private static readonly Style PurpleStyle = new TextStyle(Brushes.Purple, null, FontStyle.Regular);
        #endregion

        private void SetCPP(object sender, TextChangedEventArgs e)
        {
            e.ChangedRange.SetStyle(BrownStyle, @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'");
            e.ChangedRange.SetStyle(GreenStyle, @"//.*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(GreenStyle, @"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline);
            e.ChangedRange.SetStyle(GreenStyle, @"(/\*.*?\*/)|(.*\*/)", RegexOptions.Singleline | RegexOptions.RightToLeft);
            e.ChangedRange.SetStyle(MagentaStyle, @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");
            e.ChangedRange.SetStyle(BoldStyle, @"\b(class|struct|enum|interface)\s+(?<range>\w+?)\b");
            e.ChangedRange.SetStyle(BlueStyle, @"\b(and|and_eq|asm|auto|bitand|bitor|bool|break|case|catch|char|compl|const|const_cast|continue|default|delete|do|double|dynamic_cast|else|exit|explicit|export|extern|extern|FALSE|float|for|friend|goto|if|inline|int|long|mutable|namespace|new|not|not_eq|operator|or|or_eq|private|protected|public|register|reinterpret_cast|short|signed|sizeof|static|static_cast|string|switch|template|this|throw|TRUE|try|typedef|typeid|typename|union|unsigned|using|virtual|void|volatile|wchar_t|while|xor|xor_eq)\b");
            e.ChangedRange.SetStyle(MagentaStyle, @"#\b(include|pragma|if|else|elif|ifndef|ifdef|endif|undef|define|line|error)\b", RegexOptions.Singleline);
            e.ChangedRange.SetStyle(MagentaStyle, @"\*", RegexOptions.Singleline);
            e.ChangedRange.ClearFoldingMarkers();
            e.ChangedRange.SetFoldingMarkers("{", "}");//allow to collapse brackets block 
            e.ChangedRange.SetFoldingMarkers(@"/\*", @"\*/");//allow to collapse comment block
        }

        private void SetRPG(object sender, TextChangedEventArgs e)
        {
            //Text and comments
            e.ChangedRange.SetStyle(GreenStyle, @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'");
            e.ChangedRange.SetStyle(GreenStyle, @"//.*$", RegexOptions.Multiline);

            e.ChangedRange.SetStyle(MagentaStyle, @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");

            //Opcodes
            e.ChangedRange.SetStyle(PurpleStyle, @"\b(ACQ|ADD|ADDDUR|ALLOC|AND|BEGSR|BITOFF|BITON|CALL|CALLB|CALLP|CAT|CHAIN|CHECK|CHECKR|CLEAR|CLOSE|COMMIT|COMP|DEALLOC|DEFINE|DELETE|DIV|DO|DOU|DOW|DSPLY|DUMP|ELSE|ELSEIF|ENDDO|ENDIF|ENDSR|ENDSL|ENDMON|EVAL|EVALR|EVAL-CORR|EXCEPT|EXFMT|EXSR|EXTRCT|FEOD|FOR|FORCE|GOTO|IF|IN|ITER|KFLD|KLIST|LEAVE|LEAVESR|LOOKUP|MHHZO|MHLZO|MLHZO|MLLZO|MONITOR|MOVE|MOVEA|MOVEL|MULT|MVR|NEXT|OCCUR|ON-ERROR|OPEN|OR|OTHER|OUT|PARM|PLIST|POST|READ|READC|READE|READP|READPE|REALLOC|REL|RESET|RETURN|ROLBK|SCAN|SELECT|SETGT|SETLL|SETOFF|SETON|SHTDN|SORTA|SQRT|SUB|SUBDUR|SUBST|TAG|TEST|TESTB|TESTN|TESTZ|TIME|UNLOCK|UPDATE|WHEN|WRITE|XFOOT|XLATE|XML-INTO|XML-SAX|Z-ADD|Z-SUB)\b", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            //Types
            e.ChangedRange.SetStyle(BrownStyle, @"\b(CHAR|VARCHAR|BINDEC|FLOAT|INT|PACKED|UNS|ZONED|GRAPH|UCS2|DATE|TIME|TIMESTAMP|OBJECT|POINTER|IND)\b", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            //Dcl-*
            e.ChangedRange.SetStyle(RedStyle, @"\b(DCL-S|DCL-C|DCL-DS|DCL-F|DCL-PI|DCL-PR|CTL-OPT|DCL-PROC|END-PROC|END-DS|END-PI|END-PR)\b", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            //Directives & BIFs
            e.ChangedRange.SetStyle(BlueStyle, @"\/\b(free|end-free|copy|include|set|restore|title|define|undefine|eof|if|elseif|else|endif)\b", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            e.ChangedRange.SetStyle(BlueStyle, @"\B\%\w+", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            e.ChangedRange.ClearFoldingMarkers();
            e.ChangedRange.SetFoldingMarkers("dcl-pr", "end-pr", RegexOptions.IgnoreCase);
            e.ChangedRange.SetFoldingMarkers("dcl-pi", "end-pi", RegexOptions.IgnoreCase);
            e.ChangedRange.SetFoldingMarkers("dcl-proc", "end-proc", RegexOptions.IgnoreCase);
            e.ChangedRange.SetFoldingMarkers("begsr", "endsr", RegexOptions.IgnoreCase);
        }

    }
}
