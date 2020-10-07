using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Resource = CharaSimResource.Resource;
using WzComparerR2.Common;
using WzComparerR2.CharaSim;
using WzComparerR2.WzLib;

namespace WzComparerR2.CharaSimControl
{
    public class SkillTooltipRender2 : TooltipRender
    {
        public SkillTooltipRender2()
        {
        }

        public Skill Skill { get; set; }

        public override object TargetItem
        {
            get { return this.Skill; }
            set { this.Skill = value as Skill; }
        }

        public bool ShowProperties { get; set; } = true;
        public bool ShowDelay { get; set; }
        public bool ShowReqSkill { get; set; } = true;


        public override Bitmap Render()
        {
            if (this.Skill == null)
            {
                return null;
            }

            int picHeight;
            Bitmap originBmp = RenderSkill(out picHeight);
            Bitmap tooltip = new Bitmap(290, picHeight);
            Graphics g = Graphics.FromImage(tooltip);

            //绘制背景区域
            GearGraphics.DrawNewTooltipBack(g, 0, 0, tooltip.Width, tooltip.Height);

            //复制图像
            g.DrawImage(originBmp, 0, 0, new Rectangle(0, 0, 290, picHeight), GraphicsUnit.Pixel);

            //左上角
            g.DrawImage(Resource.UIToolTip_img_Item_Frame2_cover, 3, 3);

            if (this.ShowObjectID)
            {
                GearGraphics.DrawGearDetailNumber(g, 3, 3, Skill.SkillID.ToString("d7"), true);
            }

            if (originBmp != null)
                originBmp.Dispose();

            g.Dispose();
            return tooltip;
        }

        private Bitmap RenderSkill(out int picH)
        {
            Bitmap bitmap = new Bitmap(290, DefaultPicHeight);
            Graphics g = Graphics.FromImage(bitmap);
            StringFormat format = (StringFormat)StringFormat.GenericDefault.Clone();
            picH = 0;

            //获取文字
            StringResult sr;
            if (StringLinker == null || !StringLinker.StringSkill.TryGetValue(Skill.SkillID, out sr))
            {
                sr = new StringResultSkill();
                sr.Name = "(null)";
            }

            //绘制技能名称
            format.Alignment = StringAlignment.Center;
            TextRenderer.DrawText(g, sr.Name, GearGraphics.ItemNameFont2, new Point(bitmap.Width, 10), Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPrefix);

            //绘制图标
            if (Skill.Icon.Bitmap != null)
            {
                picH = 33;
                g.FillRectangle(GearGraphics.GearIconBackBrush2, 14, picH, 68, 68);
                g.DrawImage(GearGraphics.EnlargeBitmap(Skill.Icon.Bitmap),
                14 + (1 - Skill.Icon.Origin.X) * 2,
                picH + (33 - Skill.Icon.Bitmap.Height) * 2);
            }

            //绘制desc
            picH = 35;
            if (Skill.HyperStat)
                GearGraphics.DrawString(g, "[최대 레벨 : " + Skill.MaxLevel + "]", GearGraphics.ItemDetailFont2, 10, 274, ref picH, 16);
            else if (!Skill.PreBBSkill)
                GearGraphics.DrawString(g, "[마스터 레벨 : " + Skill.MaxLevel + "]", GearGraphics.ItemDetailFont2, 92, 274, ref picH, 16);

            if (sr.Desc != null)
            {
                string hdesc = SummaryParser.GetSkillSummary(sr.Desc, Skill.Level, Skill.Common, SummaryParams.Default);
                //string hStr = SummaryParser.GetSkillSummary(skill, skill.Level, sr, SummaryParams.Default);
                GearGraphics.DrawString(g, hdesc, GearGraphics.ItemDetailFont2, Skill.Icon.Bitmap == null ? 10 : 92, 274, ref picH, 16);
            }
            if (Skill.TimeLimited)
            {
                DateTime time = DateTime.Now.AddDays(7d);
                string expireStr = time.ToString("유효기간 : yyyy년 M월 d일 HH시 mm분");
                GearGraphics.DrawString(g, "#c" + expireStr + "#", GearGraphics.ItemDetailFont2, Skill.Icon.Bitmap == null ? 10 : 92, 274, ref picH, 16);
            }
            if (Skill.RelationSkill != null)
            {
                StringResult sr2 = null;
                if (StringLinker == null || !StringLinker.StringSkill.TryGetValue(Skill.RelationSkill.Item1, out sr2))
                {
                    sr2 = new StringResultSkill();
                    sr2.Name = "(null)";
                }
                DateTime time = DateTime.Now.AddMinutes(Skill.RelationSkill.Item2);
                string expireStr = time.ToString("유효기간 : yyyy년 M월 d일 H시 m분");
                GearGraphics.DrawString(g, "#c" + sr2.Name + "의 " + expireStr + "#", GearGraphics.ItemDetailFont2, Skill.Icon.Bitmap == null ? 10 : 92, 274, ref picH, 16);
            }
            /*if (Skill.ReqLevel > 0)
            {
                GearGraphics.DrawString(g, "#c[要求等级：" + Skill.ReqLevel.ToString() + "]#", GearGraphics.ItemDetailFont2, 90, 270, ref picH, 16);
            }
            if (Skill.ReqAmount > 0)
            {
                GearGraphics.DrawString(g, "#c" + ItemStringHelper.GetSkillReqAmount(Skill.SkillID, Skill.ReqAmount) + "#", GearGraphics.ItemDetailFont2, 90, 270, ref picH, 16);
            }*/

            //分割线
            picH = Math.Max(picH, 114);
            g.DrawLine(Pens.White, 6, picH, 283, picH);
            picH += 9;

            if (Skill.Level > 0)
            {
                string hStr = SummaryParser.GetSkillSummary(Skill, Skill.Level, sr, SummaryParams.Default);
                GearGraphics.DrawString(g, "[현재레벨 " + Skill.Level + "]", GearGraphics.ItemDetailFont, 10, 272, ref picH, 16);
                if (Skill.SkillID / 10000 / 1000 == 10 && Skill.Level == 1 && Skill.ReqLevel > 0)
                {
                    GearGraphics.DrawPlainText(g, "[필요 레벨: " + Skill.ReqLevel.ToString() + "레벨 이상]", GearGraphics.ItemDetailFont2, GearGraphics.skillYellowColor, 10, 272, ref picH, 16);
                }
                if (hStr != null)
                {
                    GearGraphics.DrawString(g, hStr, GearGraphics.ItemDetailFont2, 10, 272, ref picH, 16);
                }
            }

            if (Skill.Level < Skill.MaxLevel && !Skill.DisableNextLevelInfo)
            {
                string hStr = SummaryParser.GetSkillSummary(Skill, Skill.Level + 1, sr, SummaryParams.Default);
                GearGraphics.DrawString(g, "[다음레벨 " + (Skill.Level + 1) + "]", GearGraphics.ItemDetailFont, 10, 272, ref picH, 16);
                if (Skill.SkillID / 10000 / 1000 == 10 && (Skill.Level + 1) == 1 && Skill.ReqLevel > 0)
                {
                    GearGraphics.DrawPlainText(g, "[필요 레벨: " + Skill.ReqLevel.ToString() + "레벨 이상]", GearGraphics.ItemDetailFont2, GearGraphics.skillYellowColor, 10, 272, ref picH, 16);
                }
                if (hStr != null)
                {
                    GearGraphics.DrawString(g, hStr, GearGraphics.ItemDetailFont2, 10, 272, ref picH, 16);
                }
            }
            picH += 3;

            if (Skill.AddAttackToolTipDescSkill != 0)
            {
                g.DrawLine(Pens.White, 6, picH, 283, picH);
                picH += 9;
                GearGraphics.DrawPlainText(g, "[콤비네이션 스킬]", GearGraphics.ItemDetailFont, Color.FromArgb(119, 204, 255), 10, 272, ref picH, 16);
                BitmapOrigin icon = new BitmapOrigin();
                Wz_Node skillNode = PluginBase.PluginManager.FindWz(string.Format(@"Skill\{0}.img\skill\{1}", Skill.AddAttackToolTipDescSkill / 10000, Skill.AddAttackToolTipDescSkill));
                if (skillNode != null)
                {
                    Skill skill = Skill.CreateFromNode(skillNode, PluginBase.PluginManager.FindWz);
                    icon = skill.Icon;
                }
                if (icon.Bitmap != null)
                {
                    g.DrawImage(icon.Bitmap, 10 - icon.Origin.X, picH + 32 - icon.Origin.Y);
                }
                string skillName;
                if (this.StringLinker != null && this.StringLinker.StringSkill.TryGetValue(Skill.AddAttackToolTipDescSkill, out sr))
                {
                    skillName = sr.Name;
                }
                else
                {
                    skillName = Skill.AddAttackToolTipDescSkill.ToString();
                }
                picH += 10;
                GearGraphics.DrawString(g, skillName, GearGraphics.ItemDetailFont, 46, 272, ref picH, 16);
                picH += 6;
                picH += 8;
            }

            if (Skill.AssistSkillLink != 0)
            {
                g.DrawLine(Pens.White, 6, picH, 283, picH);
                picH += 9;
                GearGraphics.DrawPlainText(g, "[어시스트 스킬]", GearGraphics.ItemDetailFont, ((SolidBrush)GearGraphics.OrangeBrush).Color, 10, 272, ref picH, 16);
                BitmapOrigin icon = new BitmapOrigin();
                Wz_Node skillNode = PluginBase.PluginManager.FindWz(string.Format(@"Skill\{0}.img\skill\{1}", Skill.AssistSkillLink / 10000, Skill.AssistSkillLink));
                if (skillNode != null)
                {
                    Skill skill = Skill.CreateFromNode(skillNode, PluginBase.PluginManager.FindWz);
                    icon = skill.Icon;
                }
                if (icon.Bitmap != null)
                {
                    g.DrawImage(icon.Bitmap, 10 - icon.Origin.X, picH + 32 - icon.Origin.Y);
                }
                string skillName;
                if (this.StringLinker != null && this.StringLinker.StringSkill.TryGetValue(Skill.AssistSkillLink, out sr))
                {
                    skillName = sr.Name;
                }
                else
                {
                    skillName = Skill.AssistSkillLink.ToString();
                }
                picH += 10;
                GearGraphics.DrawString(g, skillName, GearGraphics.ItemDetailFont, 46, 272, ref picH, 16);
                picH += 6;
                picH += 8;
            }

            List<string> skillDescEx = new List<string>();
            if (ShowProperties)
            {
                List<string> attr = new List<string>();
                if (Skill.ReqLevel > 0)
                {
                    attr.Add("필요 레벨: " + Skill.ReqLevel);
                }
                if (Skill.Invisible)
                {
                    attr.Add("스킬창에 표시되지 않음");
                }
                if (Skill.Hyper != HyperSkillType.None)
                {
                    attr.Add("하이퍼스킬: " + Skill.Hyper);
                }
                if (Skill.CombatOrders)
                {
                    attr.Add("컴뱃오더스 적용 가능");
                }
                if (Skill.NotRemoved)
                {
                    attr.Add("버프 해제 불가");
                }
                if (Skill.MasterLevel > 0 && Skill.MasterLevel < Skill.MaxLevel)
                {
                    attr.Add("마스터리북 미사용시 마스터 레벨: Lv." + Skill.MasterLevel);
                }

                if (attr.Count > 0)
                {
                    skillDescEx.Add("#c" + string.Join(", ", attr.ToArray()) + "#");
                }
            }

            if (ShowDelay && Skill.Action.Count > 0)
            {
                foreach (string action in Skill.Action)
                {
                    skillDescEx.Add("#c[딜레이] " + action + ": " + CharaSimLoader.GetActionDelay(action) + " ms#");
                }
            }

            if (ShowReqSkill && Skill.ReqSkill.Count > 0)
            {
                foreach (var kv in Skill.ReqSkill)
                {
                    string skillName;
                    if (this.StringLinker != null && this.StringLinker.StringSkill.TryGetValue(kv.Key, out sr))
                    {
                        skillName = sr.Name;
                    }
                    else
                    {
                        skillName = kv.Key.ToString();
                    }
                    skillDescEx.Add("#c[필요 스킬] " + skillName + ": " + kv.Value + " 이상#");
                }
            }

            if (skillDescEx.Count > 0)
            {
                g.DrawLine(Pens.White, 6, picH, 283, picH);
                picH += 9;
                foreach (var descEx in skillDescEx)
                {
                    GearGraphics.DrawString(g, descEx, GearGraphics.ItemDetailFont, 10, 272, ref picH, 16);
                }
                picH += 3;
            }

            picH += 6;

            format.Dispose();
            g.Dispose();
            return bitmap;
        }
    }
}
