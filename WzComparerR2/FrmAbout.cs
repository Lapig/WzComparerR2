using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using DevComponents.AdvTree;

namespace WzComparerR2
{
    public partial class FrmAbout : DevComponents.DotNetBar.Office2007Form
    {
        public FrmAbout()
        {
            InitializeComponent();

            this.lblClrVer.Text = string.Format("{0} ({1})", Environment.Version, Environment.Is64BitProcess ? "x64": "x86");
            this.lblAsmVer.Text = GetAsmVersion().ToString();
            this.lblFileVer.Text = GetFileVersion().ToString();
            this.lblCopyright.Text = GetAsmCopyright().ToString();
            GetPluginInfo();
        }

        private Version GetAsmVersion()
        {
            return this.GetType().Assembly.GetName().Version;
        }

        private string GetFileVersion()
        {
            return this.GetAsmAttr<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? this.GetAsmAttr<AssemblyFileVersionAttribute>()?.Version;
        }

        private string GetAsmCopyright()
        {
            return this.GetAsmAttr<AssemblyCopyrightAttribute>()?.Copyright;
        }

        private void GetPluginInfo()
        {
            this.advTree1.Nodes.Clear();

            this.advTree1.Nodes.Add(new Node("KMS <font color=\"#808080\">v3.3.0</font>"));

            foreach (var contribution in new[]
            {
                new Tuple<string, string>("[KMS] 각종 기능 추가, 최종 번역", "박현민"),
                new Tuple<string, string>("[KMS] 문구 번역", "슈린냥"),
                new Tuple<string, string>("[KMS] 문구 오류 제보", "인소야닷컴 실버"),
                new Tuple<string, string>("[KMS] 문구 오류 제보", "jusir_@naver.com"),
                new Tuple<string, string>("[KMS] 장비 툴팁 오류 제보", "@Sunaries"),
                new Tuple<string, string>("[KMS] 중복 착용 불가 문구 오류 제보", "인소야닷컴 진류"),
                new Tuple<string, string>("[KMS] 아바타 저장 기능 추가", "@craftingmod"),
                new Tuple<string, string>("[KMS] 아바타 불러오기 오류 제보", "인소야닷컴 일감"),
                new Tuple<string, string>("[KMS] 파일 저장시 이름 규칙 오류 제보", "@mabooky"),
                new Tuple<string, string>("[KMS] 아바타 하이레프 귀 오류 제보", "메이플인벤 누리신드롬"),
                new Tuple<string, string>("[KMS] 각종 오류 제보, GMS 정보 제공", "@Sunaries"),
                new Tuple<string, string>("[KMS] 사용 가능 직업 문구 오류 제보", "@tanyoucai"),
                new Tuple<string, string>("[KMS] 퀘스트 상태 파티클 미적용 오류 제보", "메이플인벤 펄더"),
                new Tuple<string, string>("[KMS] 아바타 오류 제보", "@giraffebin"),
                new Tuple<string, string>("[KMS] 문구, 툴팁 위치 오류 수정 및 제보", "@OniOniOn-"),
                new Tuple<string, string>("[KMS] 패치와 함께 비교시 오류 제보", "@lowrt"),
                new Tuple<string, string>("[KMS] 아바타 모두 내보내기 오류 제보", "@pid011"),
                new Tuple<string, string>("[KMS] 툴팁 관련 기능 추가, 오류 수정 및 제보", "@sh-cho"),
            })
            {
                string nodeTxt = string.Format("{0} <font color=\"#808080\">{1}</font>",
                        contribution.Item1,
                        contribution.Item2);
                Node node = new Node(nodeTxt);
                this.advTree1.Nodes.Add(node);
            }

            if (PluginBase.PluginManager.LoadedPlugins.Count > 0)
            {
                foreach (var plugin in PluginBase.PluginManager.LoadedPlugins)
                {
                    string nodeTxt = string.Format("{0} <font color=\"#808080\">{1} ({2})</font>",
                        plugin.Instance.Name, 
                        plugin.Instance.Version,
                        plugin.Instance.FileVersion);
                    Node node = new Node(nodeTxt);
                    this.advTree1.Nodes.Add(node);
                }
            }
            else
            {
                string nodeTxt = "<font color=\"#808080\">연결된 플러그인 없음</font>";
                Node node = new Node(nodeTxt);
                this.advTree1.Nodes.Add(node);
            }
        }

        private T GetAsmAttr<T>()
        {
            object[] attr = this.GetType().Assembly.GetCustomAttributes(typeof(T), true);
            if (attr != null && attr.Length > 0)
            {
                return (T)attr[0];
            }
            return default(T);
        }
    }
}
