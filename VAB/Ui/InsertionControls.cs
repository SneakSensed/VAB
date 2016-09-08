using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;

using VAB.Builder;

namespace VAB.Ui
{
    abstract class InsertionPanel : InsertionPanelBase
    {
        protected static Color BCOLpanel = Color.FromArgb(255, 236, 233, 216);
        protected static Color BCOLbutton = Color.FromArgb(255, 30, 144, 255);
        protected static Color FCOL = Color.Black;
        protected static Dictionary<int, InsertionPanel> ipanels = new Dictionary<int, InsertionPanel>();
        protected const string CONTENT_HOLDER_NAME = "tbx_open";

        public abstract void FitResized();                                  //resize (call when form had been resized)
        public override void Propagate()
        {
            Propagates();
        }
        public abstract void Propagates();
    }

    abstract class InsertionPanelField : InsertionPanel
    {
        protected TemplateUnitBase TU;

        protected string capt;
        protected Control parentc;
        protected Label lcaption;
        protected TextBox txt;
        protected Label ljob;

        public InsertionPanelField(TemplateUnitBase tu,
                                   Control ctrl, string caption,
                                   ref int offsetY, int height, Color? colorback = null)
        {
            TU = tu;

            capt = caption;
            parentc = ctrl;
            parentc.Resize += new EventHandler((s, v) => { FitResized(); });

            this.BackColor = (colorback != null) ? colorback.Value : BCOLpanel;
            this.ForeColor = FCOL;
            this.Location = new Point(-1, offsetY);
            this.Height = height;
            this.Width = ctrl.Width + 2;

            #region Visual
            Label l = new Label();
            l.Font = new Font(Font, FontStyle.Bold);
            l.Text = capt + " : ";
            l.Width = (Int32)l.CreateGraphics().MeasureString(l.Text, l.Font).Width + 10;
            l.Location = new Point(100 - l.Width, 8);
            this.Controls.Add(l);
            lcaption = l;
            TextBox tb = new TextBox();
            tb.Name = CONTENT_HOLDER_NAME;
            tb.Font = new Font(Font, FontStyle.Bold);
            tb.BackColor = BCOLpanel;
            tb.BorderStyle = BorderStyle.FixedSingle;
            tb.Width = this.Width - 200;
            tb.Location = new Point(100, 4);
            this.Controls.Add(tb);
            txt = tb;
            Label br = new Label();
            br.Name = "open";
            br.Font = new Font("Tahoma", 8.25f, FontStyle.Bold);
            br.BackColor = BCOLbutton;
            br.Location = new Point(this.Width - 75, 4);
            br.Width = 40;
            br.Height = 20;
            br.Text = "    ...";
            br.Cursor = Cursors.Hand;
            this.Controls.Add(br);
            ljob = br;
            br.MouseClick += new MouseEventHandler(Browse_MouseClick);
            #endregion

            offsetY += this.Height;
            parentc.Controls.Add(this);
            ipanels.Add(ipanels.Count, this);
        }
        protected virtual void Browse_MouseClick(object sender, MouseEventArgs e)
        {
            Label l = ((Label)sender);
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            ofd.Title = capt;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.Controls["tbx_" + l.Name].Text = ofd.FileName;
                l.BackColor = BCOLpanel;
                l.BorderStyle = BorderStyle.FixedSingle;
            }
        }
        public override void FitResized()
        {
            this.Width = parentc.Width + 2;
            txt.Width = parentc.Width - 200;
            ljob.Location = new Point(parentc.Width - 75, ljob.Location.Y);
        }

        public override void Propagates()
        {
            PropagationMethod();
        }
        protected virtual void PropagationMethod()
        {
        }
    }
    class InsertionPanelFieldSavePath : InsertionPanelField
    {
        public InsertionPanelFieldSavePath(TemplateUnitBase tu,
                                Control ctrl, string caption,
                                ref int offsetY, int height, Color? colorback = null)
            : base(tu, ctrl, caption, ref offsetY, height, colorback)
        {
        }

        protected override void Browse_MouseClick(object sender, MouseEventArgs e)
        {
            Label l = ((Label)sender);
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            sfd.Title = capt;
            sfd.CheckFileExists = true;
            sfd.CheckPathExists = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                this.Controls["tbx_" + l.Name].Text = sfd.FileName;
                l.BackColor = BCOLpanel;
                l.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        protected override void PropagationMethod()
        {
            foreach (Control c in Controls)
            {
                if (c is TextBox && c.Name.Contains(CONTENT_HOLDER_NAME))
                {
                    (TU as TemplateUnitCs).SavePath = (c as TextBox).Text;
                    break;
                }
            }
        }
    }
    class InsertionPanelFieldIconPath : InsertionPanelField
    {
        public InsertionPanelFieldIconPath(TemplateUnitBase tu,
                                Control ctrl, string caption,
                                ref int offsetY, int height, Color? colorback = null)
            : base(tu, ctrl, caption, ref offsetY, height, colorback)
        {
        }

        protected override void PropagationMethod()
        {
            foreach (Control c in Controls)
            {
                if (c is TextBox && c.Name.Contains(CONTENT_HOLDER_NAME))
                {
                    (TU as TemplateUnitCs).IconPath = (c as TextBox).Text;
                    break;
                }
            }
        }
    }
    class InsertionPanelTemplate : InsertionPanelField, IReactable
    {
        public InsertionPanelTemplate(TemplateUnitBase tu,
                                Control ctrl,
                                ref int offsetY, int height, Color? colorback = null)
            : base(tu, ctrl, tu.Metas["varPathStored"], ref offsetY, height, colorback)
        {
            txt.Text = tu.TemplateFileName;
            txt.Enabled = false;
        }

        protected override void Browse_MouseClick(object sender, MouseEventArgs e)
        {
            ToxicForm txf = new ToxicForm(TU, this);
        }
        public void React(object o)
        {
            if (TU.IsBuilt)
            {
                this.Controls["tbx_open"].Text = TU.SavePath;
                ((Label)this.Controls["open"]).BackColor = BCOLpanel;
                ((Label)this.Controls["open"]).BorderStyle = BorderStyle.FixedSingle;
                ((Label)this.Controls["open"]).Refresh();
            }
        }
    }
    public interface IReactable
    {
        void React(object o);
    }

    abstract class InsertionPanelUnit : InsertionPanel
    {
        protected InsertionUnitBase IU;

        protected Control parentc;
        protected Label lcaption;
        protected TextBox txt;
        protected Label ljob;

        public InsertionPanelUnit(InsertionUnitBase iu,
                                  Control ctrl,
                                  ref int offsetY, int height, Color? colorback = null)
        {
            IU = iu;

            parentc = ctrl;
            parentc.Resize += new EventHandler((s, v) => { FitResized(); });

            this.BackColor = (colorback != null) ? colorback.Value : BCOLpanel;
            this.ForeColor = FCOL;
            this.Location = new Point(-1, offsetY);
            this.Height = height;
            this.Width = ctrl.Width + 2;

            #region Visual
            Label l = new Label();
            l.Font = new Font(Font, FontStyle.Bold);
            l.Text = IU.Name + " : ";
            l.Width = (Int32)l.CreateGraphics().MeasureString(l.Text, l.Font).Width + 10;
            l.Location = new Point(100 - l.Width, 8);
            this.Controls.Add(l);
            lcaption = l;
            TextBox tb = new TextBox();
            tb.Name = CONTENT_HOLDER_NAME;
            tb.Font = new Font(Font, FontStyle.Bold);
            tb.BackColor = BCOLpanel;
            tb.BorderStyle = BorderStyle.FixedSingle;
            tb.Width = this.Width - 200;
            tb.Location = new Point(100, 4);
            this.Controls.Add(tb);
            txt = tb;
            Label br = new Label();
            br.Name = "open";
            br.Font = new Font("Tahoma", 8.25f, FontStyle.Bold);
            br.BackColor = BCOLbutton;
            br.Location = new Point(this.Width - 75, 4);
            br.Width = 40;
            br.Height = 20;
            br.Text = "    ...";
            br.Cursor = Cursors.Hand;
            this.Controls.Add(br);
            ljob = br;
            br.MouseClick += new MouseEventHandler(Browse_MouseClick);
            #endregion

            offsetY += this.Height;
            parentc.Controls.Add(this);
            ipanels.Add(ipanels.Count, this);
        }
        protected virtual void Browse_MouseClick(object sender, MouseEventArgs e)
        {
            Label l = ((Label)sender);
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            ofd.Title = IU.Name;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.Controls["tbx_" + l.Name].Text = ofd.FileName;
                l.BackColor = BCOLpanel;
                l.BorderStyle = BorderStyle.FixedSingle;
            }
        }
        public override void FitResized()
        {
            this.Width = parentc.Width + 2;
            txt.Width = parentc.Width - 200;
            ljob.Location = new Point(parentc.Width - 75, ljob.Location.Y);
        }

        public override void Propagates()
        {
            PropagationMethod();
        }
        protected virtual void PropagationMethod()
        {
        }
    }
    class InsertionPanelString : InsertionPanelUnit
    {
        public InsertionPanelString(InsertionUnit<string> iu,
                                Control ctrl,
                                ref int offsetY, int height, Color? colorback = null)
            : base(iu, ctrl, ref offsetY, height, colorback)
        {
        }

        protected override void PropagationMethod()
        {
            foreach (Control c in Controls)
            {
                if (c is TextBox && c.Name.Contains(CONTENT_HOLDER_NAME))
                {
                    (IU as InsertionUnit<string>).Fill = (c as TextBox).Text;
                    break;
                }
            }
        }
    }

    abstract class InsertionPanelUnits : InsertionPanel
    {
        protected InsertionUnitBase IU;

        protected Control parentc;
        protected Label lcaption;
        protected List<TextBox> litxt = new List<TextBox>();
        protected List<Label> liljob = new List<Label>();
        protected List<Label> lilplus = new List<Label>();
        protected int numberoflines = 1;
        protected int lineHeight;

        public InsertionPanelUnits(InsertionUnitBase iu,
                                  Control ctrl,
                                  ref int offsetY, int height, Color? colorback = null)
        {
            IU = iu;

            parentc = ctrl;
            parentc.Resize += new EventHandler((s, v) => { FitResized(); });

            this.BackColor = (colorback != null) ? colorback.Value : BCOLpanel;
            this.ForeColor = FCOL;
            this.Location = new Point(-1, offsetY);
            this.Height = height;
            lineHeight = height;
            this.Width = ctrl.Width + 2;

            #region Visual
            Label l = new Label();
            l.Font = new Font(Font, FontStyle.Bold);
            l.Text = IU.Name + " : ";
            l.Width = (Int32)l.CreateGraphics().MeasureString(l.Text, l.Font).Width + 10;
            l.Location = new Point(100 - l.Width, 8);
            this.Controls.Add(l);
            lcaption = l;
            TextBox tb = new TextBox();
            tb.Name = CONTENT_HOLDER_NAME;
            tb.Font = new Font(Font, FontStyle.Bold);
            tb.BackColor = BCOLpanel;
            tb.BorderStyle = BorderStyle.FixedSingle;
            tb.Width = this.Width - 200;
            tb.Location = new Point(100, 4);
            this.Controls.Add(tb);
            litxt.Add(tb);
            Label br = new Label();
            br.Name = "open";
            br.Font = new Font("Tahoma", 8.25f, FontStyle.Bold);
            br.BackColor = BCOLbutton;
            br.Location = new Point(this.Width - 75, 4);
            br.Width = 40;
            br.Height = 20;
            br.Text = "    ...";
            br.Cursor = Cursors.Hand;
            this.Controls.Add(br);
            liljob.Add(br);
            br.MouseClick += new MouseEventHandler(Browse_MouseClick);
            Label plus = new Label();
            plus.Name = "plus";
            plus.Font = new Font(Font, FontStyle.Bold);
            plus.BackColor = BCOLpanel;
            plus.Location = new Point(this.Width - 20, 4);
            plus.Width = 20;
            plus.Height = 20;
            plus.Text = " + ";
            plus.Cursor = Cursors.Hand;
            this.Controls.Add(plus);
            lilplus.Add(plus);
            plus.Click += new EventHandler(plus_Click);
            #endregion

            offsetY += this.Height;
            parentc.Controls.Add(this);
            ipanels.Add(ipanels.Count, this);
        }
        protected virtual void Browse_MouseClick(object sender, MouseEventArgs e)
        {
            Label l = ((Label)sender);
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            ofd.Title = IU.Name;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.Controls["tbx_" + l.Name].Text = ofd.FileName;
                l.BackColor = BCOLpanel;
                l.BorderStyle = BorderStyle.FixedSingle;
            }
        }
        void plus_Click(object sender, EventArgs e)
        {
            TextBox tb = new TextBox();
            Name = "tbx_open" + (numberoflines + 1);
            tb.Font = new Font(Font, FontStyle.Bold);
            tb.BackColor = BCOLpanel;
            tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tb.Width = this.Width - 200;
            tb.Location = new Point(100, (lineHeight * numberoflines) + 2);
            this.Controls.Add(tb);
            litxt.Add(tb);
            Label br = new Label();
            br.Name = "open" + (numberoflines + 1);
            br.Font = new Font("Tahoma", 8.25f, FontStyle.Bold);
            br.BackColor = BCOLbutton;
            br.Width = 40;
            br.Height = 20;
            br.Text = "    ...";
            br.Cursor = Cursors.Hand;
            this.Controls.Add(br);
            liljob.Add(br);
            br.Location = new Point(this.Width - 75, (lineHeight * numberoflines) + 2);
            br.MouseClick += new MouseEventHandler(Browse_MouseClick);

            this.Height += (lineHeight);
            try
            {
                ToxicFormMain txf = (ToxicFormMain)this.FindForm();
                txf.OffsetY += lineHeight;
                txf.MinimumSize = new Size(txf.MinimumSize.Width, txf.MinimumSize.Height + lineHeight);
                txf.MaximumSize = new Size(txf.MaximumSize.Width, txf.MaximumSize.Height + lineHeight);
                txf.Height += lineHeight;
                txf.BuildButton.FitResized();
            }
            catch { }
            int myKey = ipanels.FirstOrDefault(x => x.Value == this).Key;
            for (int i = myKey + 1; i < ipanels.Count; i++)
            {
                ipanels[i].Location = new Point(ipanels[i].Location.X, ipanels[i].Location.Y + lineHeight);
            }
            foreach (InsertionPanel ip in ipanels.Values)
            {
                ip.FitResized();
            }
            numberoflines++;
            FitResized();
        }
        public override void FitResized()
        {
            this.Width = parentc.Width + 2;
            foreach (TextBox t in litxt)
            {
                t.Width = parentc.Width - 200;
            }
            foreach (Label l in liljob)
            {
                l.Location = new Point(parentc.Width - 75, l.Location.Y);
            }
            foreach (Label l in lilplus)
            {
                l.Location = new Point(parentc.Width - 20, l.Location.Y);
            }
        }

        public override void Propagates()
        {
            PropagationMethod();
        }
        protected virtual void PropagationMethod()
        {
        }
    }
    class InsertionPanelListString : InsertionPanelUnits
    {
        public InsertionPanelListString(InsertionUnit<List<string>> iu,
                                Control ctrl,
                                ref int offsetY, int height, Color? colorback = null)
            : base(iu, ctrl, ref offsetY, height, colorback)
        {
        }

        protected override void PropagationMethod()
        {
            foreach (Control c in Controls)
            {
                if (c is TextBox && c.Name.Contains(CONTENT_HOLDER_NAME))
                {
                    (IU as InsertionUnit<List<string>>).Fill.Add((c as TextBox).Text);
                }
            }
        }
    }


    class OrangeButton : Button
    {
        Control parentc;
        const int DISTANCE = 10;

        public OrangeButton(Control ctrl, ref int offsetY, int height)
        {
            parentc = ctrl;
            parentc.Resize += new EventHandler((s, v) => { FitResized(); });
            parentc.FindForm().ResizeEnd += new EventHandler((s, v) => { FitResized(); });

            ForeColor = Color.Black;
            Text = "Build";
            Height = height;
            Font = new Font("Tahoma", 9.25f);
            Width = (Int32)this.CreateGraphics().MeasureString(Text, Font).Width + 30;
            ctrl.Controls.Add(this);

            try
            {
                ((ToxicFormMain)this.FindForm()).OffsetY += DISTANCE;
                int y = ((ToxicFormMain)this.FindForm()).OffsetY + DISTANCE;
                int x = (parentc.Width - Width) / 2;
                Location = new Point(x, y);
            }
            catch
            {
                ((ToxicForm)this.FindForm()).OffsetY += DISTANCE;
                int y = ((ToxicForm)this.FindForm()).OffsetY + DISTANCE;
                int x = (parentc.Width - Width) / 2;
                Location = new Point(x, y);
            }

        }
        public void FitResized()
        {
            try
            {
                int y = ((ToxicFormMain)this.FindForm()).OffsetY + DISTANCE;
                int x = (parentc.Width - Width) / 2;
                Location = new Point(x, y);
            }
            catch
            {
                int y = ((ToxicForm)this.FindForm()).OffsetY + DISTANCE;
                int x = (parentc.Width - Width) / 2;
                Location = new Point(x, y);
            }
        }
    }
}