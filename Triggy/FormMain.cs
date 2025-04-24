using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace triggy
{
    public partial class FormMain : Form
    {
        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                foreach (TextBox t in _recentTextBoxes)
                {
                    // remove keypress and enter event handlers
                    t.KeyPress -= TextBox_KeyPress;
                    t.Enter -= Textbox_Enter;
                }

                // Dispose of _triangle and _workarea
                _triangle?.Dispose();
                _workarea?.Dispose();

                components.Dispose();
            }
            base.Dispose(disposing);
        }


        enum TextboxType
        {
            AngleA,
            SideC,
            AngleB,
            SideA,
            AngleC,
            SideB
        }

        /// <summary>
        /// Textbox names to describe textbox (without space)
        /// </summary>
        struct TextboxNames
        {
            const string SideA = "SideA";
            const string AngleA = "AngleA";
            const string SideB = "SideB";
            const string AngleB = "AngleB";
            const string SideC = "SideC";
            const string AngleC = "AngleC";

            static public string[] ToArray()
            {
                return [AngleA, SideC, AngleB, SideA, AngleC, SideB];
                // return [SideA, AngleA, SideB, AngleB, SideC, AngleC];
            }
        }

        /// <summary>
        /// Label captions to describe each textbox (with space)
        /// </summary>
        struct LabelCaptions
        {
            const string SideA = "Side a";
            const string AngleA = "Angle A";
            const string SideB = "Side b";
            const string AngleB = "Angle B";
            const string SideC = "Side c";
            const string AngleC = "Angle C";

            static public string[] ToArray()
            {
                return [AngleA, SideC, AngleB, SideA, AngleC, SideB];
            }
        }

        // define form-level layout constants
        internal readonly TextBox _txtSideA;
        internal readonly TextBox _txtSideB;
        internal readonly TextBox _txtSideC;
        internal readonly TextBox _txtAngleA;
        internal readonly TextBox _txtAngleB;
        internal readonly TextBox _txtAngleC;

        internal readonly Label _lblSideA;
        internal readonly Label _lblSideB;
        internal readonly Label _lblSideC;
        internal readonly Label _lblAngleA;
        internal readonly Label _lblAngleB;
        internal readonly Label _lblAngleC;

        internal readonly Button _btnSolve;
        internal readonly Button _btnClear;

        internal readonly ListBox _listBox;

        // make list of textboxes
        private readonly List<TextBox> _recentTextBoxes = new List<TextBox>();
        private readonly Label[] _labels;

        // define opposing pairs by linking with textboxes
        private readonly OpposingPair _a;
        private readonly OpposingPair _b;
        private readonly OpposingPair _c;

        // need a form-level triangle which can be passed to the sizechagned event
        private readonly TriangleSolver _triangle;

        /// <summary>
        /// Bounds for displaying triangle
        /// </summary>
        private readonly TriangleWorkarea _workarea;

        public FormMain()
        {
            InitializeComponent();

            // force form size to begin
            FormBorderStyle = FormBorderStyle.Sizable;
            Text = "Triggy the Love Triangle";
            Width = FRM.WIDTH;
            Height = FRM.HEIGHT;
            MinimumSize = new Size(FRM.WIDTH, FRM.HEIGHT);
            StartPosition = FormStartPosition.CenterScreen;


            // define and position each textbox to suit its puropse
            SetupTextbox(out _txtSideA, out _lblSideA, this, TextboxType.SideA);
            SetupTextbox(out _txtAngleA, out _lblAngleA, this, TextboxType.AngleA);
            SetupTextbox(out _txtSideB, out _lblSideB, this, TextboxType.SideB);
            SetupTextbox(out _txtAngleB, out _lblAngleB, this, TextboxType.AngleB);
            SetupTextbox(out _txtSideC, out _lblSideC, this, TextboxType.SideC);
            SetupTextbox(out _txtAngleC, out _lblAngleC, this, TextboxType.AngleC);

            // set array of textboxes once defined
            // start with Angle A, Angle B and Side C as the default primary textboxes
            _recentTextBoxes = [_txtAngleA, _txtAngleB, _txtSideC, _txtSideA, _txtSideB, _txtAngleC];
            _labels = [_lblAngleA, _lblAngleB, _lblSideC, _lblSideA, _lblSideB, _lblAngleC];

            foreach (TextBox txtbox in _recentTextBoxes)
            {
                // add each to form
                Controls.Add(txtbox);
            }

            foreach (Label lbl in _labels)
            {
                // add each to form
                Controls.Add(lbl);
            }

            // define and position button
            SetupButton(out _btnSolve, "btnSolve", "Solve");
            SetupButton(out _btnClear, "btnClear", "Clear", _btnSolve);
            SetupListBox(out _listBox);

            // set up acceptbutton (return accept)
            AcceptButton = _btnSolve;

            // add to form
            Controls.Add(_btnSolve);
            Controls.Add(_btnClear);
            Controls.Add(_listBox);

            // define opposing pairs by assigning textboxes
            _a = new(_txtAngleA, _txtSideA);
            _b = new(_txtAngleB, _txtSideB);
            _c = new(_txtAngleC, _txtSideC);

            // define triangle with links to textboxes
            _triangle = new(_a, _b, _c);

            // define hatched workarea without a triangle
            _workarea = new(this);

            // import triangle into workarea
            _workarea.ImportTriangle(_triangle);

            // force OnPaint to redraw the triangle
            Invalidate();

        }

        /// <summary>
        /// Set up initial button
        /// </summary>
        /// <param name="button"></param>
        /// <param name="name"></param>
        /// <param name="text"></param>
        private void SetupButton(out Button button, string name, string text)
        {
            button = new Button() { Name = name, Text = text, Height = BTN.HEIGHT, Width = BTN.WIDTH, Top = FRM.BORDER, Left = FRM.BORDER };
            button.Click += Button_Click;
        }

        /// <summary>
        /// Set up additional button located beneath <paramref name="btnAbove"/>
        /// </summary>
        /// <param name="button"></param>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <param name="btnAbove"></param>
        private void SetupButton(out Button button, string name, string text, Button btnAbove)
        {
            button = new Button() { Name = name, Text = text, Height = BTN.HEIGHT, Width = BTN.WIDTH, Top = btnAbove.Top + btnAbove.Height + FRM.BORDER, Left = FRM.BORDER };
            button.Click += Button_Click;
        }

        private void SetupListBox(out ListBox listbox)
        {
            // retrieve font, change size and style
            Font font = new("Courier New", 8, FontStyle.Regular, GraphicsUnit.Point);
            listbox = new ListBox() { Width = BTN.WIDTH, Top = FRM.BORDER, Left = ClientRectangle.Width - BTN.WIDTH - FRM.BORDER, Font = font, IntegralHeight = false };
            listbox.Height = listbox.ItemHeight * 12;
            listbox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        }

        /// <summary>
        /// Define and position all textboxes
        /// </summary>
        /// <param name="txtbox"></param>
        /// <param name="form"></param>
        /// <param name="position"></param>
        private void SetupTextbox(out TextBox txtbox, out Label lbl, Form form, TextboxType position)
        {
            string name = TextboxNames.ToArray()[(int)position];
            string caption = LabelCaptions.ToArray()[(int)position];

            // define textbox and set properties
            txtbox = new TextBox() { Name = name, Height = TXT.HEIGHT, Width = TXT.WIDTH };

            // retrieve font, change size and style
            Font font = new(txtbox.Font.FontFamily.Name, 12, FontStyle.Bold, GraphicsUnit.Point);

            // define label and apply properties
            lbl = new Label() { Name = "lbl" + name, Text = caption, Font = font, Width = 70, ForeColor = Color.SteelBlue };

            // set keypress and enter event handlers
            txtbox.KeyPress += TextBox_KeyPress;
            txtbox.Enter += Textbox_Enter;
            txtbox.Leave += Textbox_Leave;
            txtbox.Font = font;

            // set tab order around the circle
            txtbox.TabIndex = (int)position;

            switch (position)
            {
                case TextboxType.SideA:
                    {
                        txtbox.Top = form.ClientRectangle.Height / 2 - txtbox.Height / 2;
                        txtbox.Left = FRM.BORDER;
                        txtbox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                        txtbox.MaxLength = 8; // 99999.99

                        // position label above
                        lbl.Top = txtbox.Top - lbl.Height - FRM.BORDER;
                        lbl.Left = txtbox.Left;
                        lbl.Anchor = txtbox.Anchor;

                        break;
                    }
                case TextboxType.AngleA:
                    {
                        txtbox.Top = form.ClientRectangle.Height - FRM.BORDER - txtbox.Height;
                        txtbox.Left = form.ClientRectangle.Width - txtbox.Width - FRM.BORDER;
                        txtbox.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                        txtbox.MaxLength = 6; // 179.99

                        // position label above
                        lbl.Top = txtbox.Top - lbl.Height - FRM.BORDER;
                        lbl.Left = txtbox.Left;
                        lbl.Anchor = txtbox.Anchor;

                        break;
                    }
                case TextboxType.SideB:
                    {
                        txtbox.Top = form.ClientRectangle.Height / 2 - txtbox.Height / 2;
                        txtbox.Left = form.ClientRectangle.Width - txtbox.Width - FRM.BORDER;
                        txtbox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                        txtbox.MaxLength = 8; // 99999.99

                        // position label above
                        lbl.Top = txtbox.Top - lbl.Height - FRM.BORDER;
                        lbl.Left = txtbox.Left;
                        lbl.Anchor = txtbox.Anchor;

                        break;
                    }
                case TextboxType.AngleB:
                    {
                        txtbox.Top = form.ClientRectangle.Height - FRM.BORDER - txtbox.Height;
                        txtbox.Left = FRM.BORDER;
                        txtbox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                        txtbox.MaxLength = 6; // 179.99

                        // position label above
                        lbl.Top = txtbox.Top - lbl.Height - FRM.BORDER;
                        lbl.Left = txtbox.Left;
                        lbl.Anchor = txtbox.Anchor;

                        break;
                    }
                case TextboxType.SideC:
                    {
                        txtbox.Top = form.ClientRectangle.Height - FRM.BORDER - txtbox.Height;
                        txtbox.Left = form.ClientRectangle.Width / 2 - txtbox.Width / 2;
                        txtbox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                        txtbox.MaxLength = 8; // 99999.99

                        // position label beside
                        lbl.Top = txtbox.Top;
                        lbl.Left = txtbox.Left - lbl.Width - FRM.BORDER;
                        lbl.Anchor = txtbox.Anchor;

                        break;
                    }

                case TextboxType.AngleC:
                    {
                        txtbox.Top = FRM.BORDER;
                        txtbox.Left = form.ClientRectangle.Width / 2 - txtbox.Width / 2;
                        txtbox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                        txtbox.MaxLength = 6; // 179.99

                        // position label beside
                        lbl.Top = txtbox.Top;
                        lbl.Left = txtbox.Left - lbl.Width - FRM.BORDER;
                        lbl.Anchor = txtbox.Anchor;

                        break;
                    }
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// handle button click to perform specific actions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object? sender, EventArgs e)
        {

            if (sender == _btnClear)
            {
                // clear should just empty textboxes and then run solve

                // solve on empty should be same as clear

                // i need to determine insufficient triangle data and clear remaining data

                // two angles or any three fields is sufficient

                // if a triangle cannnot be made, 1 side 1 angle, then nothing happens and workarea is cleared

                // alternate triangle still exists when main triangle does not


                for (int i = 0; i < _recentTextBoxes.Count; i++)
                {
                    // clear each textbox
                    _recentTextBoxes[i].Text = string.Empty;
                }

                // define new triangle using current textbox values
                _triangle.GetTextBoxValues();

                // import triangle into workarea
                _workarea.ImportTriangle(_triangle);

                // force OnPaint to redraw the triangle
                Invalidate();

                // display all items in list
                _listBox.Items.Clear();

                // focus on first textbox
                _recentTextBoxes[0].Select();
            }
            else
            {
                // clear the oldest 3 textboxes
                for (int i = 3; i < _recentTextBoxes.Count; i++)
                {
                    // clear each textbox
                    _recentTextBoxes[i].Text = string.Empty;
                }

                // define new triangle using current textbox values
                _triangle.GetTextBoxValues();


                List<string> resultMessages = new();

                // try to solve and store result
                if (_triangle.SolveSSA(ref resultMessages))
                {
                    Debug.WriteLine(" Solved Side Side Angle");

                    // store original side lengths for dynamic scaling
                    //_triangle.SetOriginalSides(a, b, c);
                }
                else if (_triangle.SolveASA(ref resultMessages))
                {
                    Debug.WriteLine(" Solved Angle Side Angle");

                    // store original side lengths for dynamic scaling
                    //_triangle.SetOriginalSides(a, b, c);
                }
                else if (_triangle.SolveSAS(ref resultMessages))
                {
                    Debug.WriteLine(" Solved Side Angle Side");

                    // fails if side = 50, angle = 60, side = 15

                    // store original side lengths for dynamic scaling
                    //_triangle.SetOriginalSides(a, b, c);
                }
                else if (_triangle.SolveSSS(ref resultMessages))
                {
                    Debug.WriteLine(" Solved Side Side Side");

                    // store original side lengths for dynamic scaling
                    //_triangle.SetOriginalSides(a, b, c);
                }

                if (resultMessages.Count > 0)
                {
                    // show one messagebox with each error on a new line
                    MessageBox.Show(this, string.Join(Environment.NewLine, resultMessages), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }



            // display completed triangle results in all textboxes
            _triangle.SetTextBoxValues();

            // import triangle into workarea
            _workarea.ImportTriangle(_triangle);

            // force OnPaint to redraw the triangle
            Invalidate();

            // display all items in list
            _listBox.Items.Clear();
            _listBox.Items.AddRange(_triangle.GetResultList());


        }

        /// <summary>
        /// Arrange textboxes in list and colour text black or grey accordingly
        /// </summary>
        /// <param name="sender"></param>
        private void TextBox_EnterLeave(object sender)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox == null) return;

            // only set as primary if not empty
            if (!textBox.Empty() && textBox.ToDouble() > 0)
            {
                // Remove if it already exists
                _recentTextBoxes.Remove(textBox);

                // Insert at the beginning (top of the list)
                _recentTextBoxes.Insert(0, textBox);
            }
            else
            {
                // move to bottom if empty or zero
                if (textBox.Empty() || textBox.ToDouble() == 0)
                {
                    // Remove if it already exists
                    _recentTextBoxes.Remove(textBox);

                    // Insert at the end (bottom of the list)
                    _recentTextBoxes.Add(textBox);
                }
            }

            // update textbox colours to suit position
            for (int i = 0; i < _recentTextBoxes.Count; i++)
            {
                if (i < 3)
                {
                    // black for primary 3
                    _recentTextBoxes[i].ForeColor = SystemColors.WindowText;
                }
                else
                {
                    // grey for secondary 3
                    _recentTextBoxes[i].ForeColor = Color.DarkGray;
                }
            }

        }



        /// <summary>
        /// handle keypress events to ensure only numbers or single decimal place is permitted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Allow digits (0-9) and control characters (e.g., backspace, delete)
                if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
                {
                    // set textbox to top of list and colour text black
                    TextBox_EnterLeave(sender);

                    return; // Allow the input
                }

                // Allow a single decimal point, but only if it doesn't already exist
                if (e.KeyChar == '.' && !textBox.Text.Contains('.'))
                {
                    // set textbox to top of list and colour text black
                    TextBox_EnterLeave(sender);

                    return; // Allow the input
                }

                // If the input is not valid, suppress it
                e.Handled = true;
            }
        }

        // handle textbox enter events to track last 3 focussed textboxes
        private void Textbox_Enter(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                // set textbox to top of list and colour text black
                TextBox_EnterLeave(sender);
            }
        }

        private void Textbox_Leave(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                // set setbox to bottom of list and colour text grey
                TextBox_EnterLeave(sender);
            }
        }

        /// <summary>
        /// Override OnResize to reposition textboxes and redraw triangle (OnPaint)
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (IsHandleCreated)
            {
                // reposition textboxes while resizing
                _txtSideC.Left = ClientRectangle.Width / 2 - _txtSideC.Width / 2;
                _txtAngleC.Left = ClientRectangle.Width / 2 - _txtAngleC.Width / 2;

                _txtSideA.Top = ClientRectangle.Height / 2 - _txtSideA.Height / 2;
                _txtSideB.Top = ClientRectangle.Height / 2 - _txtSideB.Height / 2;

                // reposition labels relative to their textboxes
                _lblSideC.Left = _txtSideC.Left - _lblSideC.Width - FRM.BORDER;
                _lblSideC.Top = _txtSideC.Top;

                _lblAngleC.Left = _txtAngleC.Left - _lblAngleC.Width - FRM.BORDER;
                _lblAngleC.Top = _txtAngleC.Top;

                _lblSideA.Left = _txtSideA.Left;
                _lblSideA.Top = _txtSideA.Top - _lblSideA.Height - FRM.BORDER;

                _lblSideB.Left = _txtSideB.Left;
                _lblSideB.Top = _txtSideB.Top - _lblSideB.Height - FRM.BORDER;

                _lblAngleA.Left = _txtAngleA.Left;
                _lblAngleA.Top = _txtAngleA.Top - _lblAngleA.Height - FRM.BORDER;

                _lblAngleB.Left = _txtAngleB.Left;
                _lblAngleB.Top = _txtAngleB.Top - _lblAngleB.Height - FRM.BORDER;

                // force OnPaint to redraw the triangle
                Invalidate();
            }

        }

        /// <summary>
        /// Override OnPaint to draw the triangle
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_workarea != null)
            {
                Graphics g = e.Graphics;

                // using statements for classes that implement the iDisposable interface to ensure correct garbage collection

                // fill display bounds
                using (Brush brush = new HatchBrush(HatchStyle.Cross, Color.White, Color.Lavender))
                {
                    g.FillRectangle(brush, _workarea.Bounds);
                }

                // Draw display bounds
                using (Pen pen = new(Color.MediumPurple, 1))
                {
                    g.DrawRectangle(pen, _workarea.Bounds);
                }

                // draw main triangle
                if (!_workarea.MainPoints.IsEmpty)
                {
                    // fill triangle
                    using (Brush brush = new HatchBrush(HatchStyle.SmallCheckerBoard, Color.AliceBlue, Color.SteelBlue))
                    {
                        g.FillPolygon(brush, _workarea.MainPoints.ToArray());
                    }

                    // draw triangle outline
                    using (Pen pen = new(Color.MidnightBlue, 3))
                    {
                        // Draw triangle
                        g.DrawPolygon(pen, _workarea.MainPoints.ToArray());
                    }
                }

                // draw alternate triangle
                if (!_workarea.AltPoints.IsEmpty)
                {
                    //// fill triangle
                    //using (Brush brush = new HatchBrush(HatchStyle.SmallCheckerBoard, Color.White, Color.Transparent))
                    //{
                    //    g.FillPolygon(brush, _workarea.AltPoints.ToArray());
                    //}

                    // draw triangle outline
                    using (Pen pen = new(Color.MidnightBlue, 1))
                    {
                        // Draw triangle
                        g.DrawPolygon(pen, _workarea.AltPoints.ToArray());
                    }
                }

            }
        }



    }
}
