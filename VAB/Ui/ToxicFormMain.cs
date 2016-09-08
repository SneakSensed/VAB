using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

using VAB.Builder;

namespace VAB.Ui
{
    public partial class ToxicFormMain : ToxicFormBase
    {
        const string DEFTITLE = "VAB 1.0.0.0";
        const string DEFTEXT = "VAB";

        int hp, wp;
        Control canvas;
        internal OrangeButton BuildButton;
        public ToxicFormMain(TemplateUnitBase tu,
                        string text = "", string title = "") : base(tu)
        {
            InitializeComponent();

            #region visual
            this.Text = text == "" ? DEFTEXT : text;
            //title
            Rectangle r = Screen.FromControl(this).Bounds;
            hp = r.Height / 100;
            wp = r.Width / 100;

            this.MinimumSize = new Size(wp * 80, hp * 50);
            Icon = new System.Drawing.Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("VAB.Ui.icon.ico"));
            Panel back = new Panel();
            back.Dock = DockStyle.Fill;
            var rrr = this.BackColor;
            this.Controls.Add(back);
            Panel P = new Panel();
            P.Name = "OuR Canvas";
            P.Height = 10000;
            P.Width = back.Width;
            P.Location = new Point(0, 0);
            P.MouseDown += new MouseEventHandler(P_MouseDown);
            P.MouseMove += new MouseEventHandler(P_MouseMove);
            P.MouseUp += new MouseEventHandler(P_MouseUp);
            back.Controls.Add(P);
            canvas = P;
            this.Resize += new EventHandler((s, v) =>
            {
                back.Height = this.ClientRectangle.Height - 66;
                back.Width = this.ClientRectangle.Width - 13;
                canvas.Width = back.Width;
            });

            Width = wp * 70;
            Height = hp * 60;
            MinimumSize = new System.Drawing.Size(600, Height);
            MaximumSize = new System.Drawing.Size(10000, Height);
            #endregion

            this.Load += new EventHandler(ToxicForm_Load);
            TU = tu;

            try 
            {
                System.Windows.Forms.Application.Run(this); 
            }
            catch 
            {
                try
                {
                    Show();
                }
                catch { }
            }
        }

        void ToxicForm_Load(object sender, EventArgs e)
        {
            Populate();
        }
        #region visualMethods
        private bool _dragging = false;
        private Point _start_point = new Point(0, 0);
        void P_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }
        void P_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point p = e.Location;
                int change = (p.Y - this._start_point.Y);
                if (((Panel)sender).Location.Y + change <= 0)
                {
                    ((Panel)sender).Location = new Point(0, ((Panel)sender).Location.Y + change);
                }
            }
        }
        void P_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _start_point = new Point(0, e.Y);
        }
        void Build_Click(object sender, EventArgs e)
        {
            Propagate();
            if (TU is TemplateUnitCs) (TU as TemplateUnitCs).Build();
            if (TU is TemplateUnitTxt) (TU as TemplateUnitTxt).Build();
        }
        #endregion


        TemplateUnitBase TU;

        internal int OffsetY = 5;
        List<InsertionPanel> insertionsPanels = new List<InsertionPanel>();
        List<InsertionPanel> templatesPanels = new List<InsertionPanel>();

        public override void Populate()
        {
            insertionsPanels.Add(new InsertionPanelFieldSavePath(TU as TemplateUnitCs, canvas, "Save Path", ref OffsetY, hp * 4));
            insertionsPanels.Add(new InsertionPanelFieldIconPath(TU as TemplateUnitCs, canvas, "Icon Path", ref OffsetY, hp * 4));

            OffsetY += 10;
            foreach (TemplateUnitBase t in TU.Templates)
                templatesPanels.Add(new InsertionPanelTemplate(t as TemplateUnitBase, canvas, ref OffsetY, hp * 4));

            OffsetY += 10;
            foreach (InsertionUnitBase u in TU.Insertions)
            {
                if (u is InsertionUnit<List<string>>)
                    insertionsPanels.Add(new InsertionPanelListString(u as InsertionUnit<List<string>>, canvas, ref OffsetY, hp * 4));
                else if (u is InsertionUnit<string>)
                    insertionsPanels.Add(new InsertionPanelString(u as InsertionUnit<string>, canvas, ref OffsetY, hp * 4));
            }
            BuildButton = new OrangeButton(canvas, ref OffsetY, hp * 4);
            BuildButton.Click += new EventHandler(Build_Click);
            BuildButton.FlatStyle = FlatStyle.Popup;
        }
        public override void Propagate()
        {
            foreach (InsertionPanel ipanel in insertionsPanels)
            {
                ipanel.Propagate();
            }
            foreach (InsertionPanel tpanel in templatesPanels)
            {
                tpanel.Propagate();
            }
            TU.Build();
        }
    }
}
