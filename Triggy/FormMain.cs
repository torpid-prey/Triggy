using System.Diagnostics; // Debug.WriteLine
using System.Drawing.Drawing2D; // Rectangles and Polygons
using System.Text; // StringBuilder
using System.Text.RegularExpressions; // Regex
using triggy.Properties; // Resource.Images

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
                    // remove textbox event handlers
                    t.KeyPress -= TextBox_KeyPress;
                    t.Enter -= Textbox_Enter;
                    t.Leave -= Textbox_Leave;
                    t.KeyUp -= TextBox_KeyUp;
                }

                // remove button event handlers
                _btnSolve.Click -= Button_Click;
                _btnClear.Click -= Button_Click;
                _btnRotate.Click -= Button_Click;
                _btnSave.Click -= Button_Click;
                _btnLoad.Click -= Button_Click;

                // Dispose of _triangle and _workarea
                _triangle?.Dispose();
                _workarea?.Dispose();

                // dispose of non-control components
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
                // start with bottom three inputs at top of priority list
                return [AngleA, SideC, AngleB, SideA, AngleC, SideB];
            }
        }

        /// <summary>
        /// Label captions to describe each textbox (with space)
        /// </summary>
        struct LabelCaptions
        {
            const string SideA = "Side a";
            const string AngleA = "Angle A°";
            const string SideB = "Side b";
            const string AngleB = "Angle B°";
            const string SideC = "Side c";
            const string AngleC = "Angle C°";

            static public string[] ToArray()
            {
                // same order as textboxes duh
                return [AngleA, SideC, AngleB, SideA, AngleC, SideB];
            }
        }

        struct ButtonNames
        {
            public const string btnSolve = "btnSolve";
            public const string btnClear = "btnClear";
            public const string btnRotate = "btnRotate";
            public const string btnSave = "btnSave";
            public const string btnLoad = "btnLoad";
        }

        // constant file extension
        internal const string FILEEXT = ".txt";

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
        internal readonly Button _btnRotate;
        internal readonly Button _btnSave;
        internal readonly Button _btnLoad;

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

        /// <summary>
        /// Resources byte array returned as image
        /// </summary>
        private static Image ImageSolve
        {
            get
            {
                Image result;
                using (MemoryStream stream = new MemoryStream(Resources.imgSolve))
                {
                    result = Image.FromStream(stream);
                }
                return result;
            }
        }

        /// <summary>
        /// Resources byte array returned as image
        /// </summary>
        private static Image ImageClear
        {
            get
            {
                Image result;
                using (MemoryStream stream = new MemoryStream(Resources.imgClear))
                {
                    result = Image.FromStream(stream);
                }
                return result;
            }
        }

        /// <summary>
        /// Resources byte array returned as image
        /// </summary>
        private static Image ImageRotate
        {
            get
            {
                Image result;
                using (MemoryStream stream = new MemoryStream(Resources.imgRotate))
                {
                    result = Image.FromStream(stream);
                }
                return result;
            }
        }

        /// <summary>
        /// Resources byte array returned as image
        /// </summary>
        private static Image ImageSave
        {
            get
            {
                Image result;
                using (MemoryStream stream = new MemoryStream(Resources.imgSave))
                {
                    result = Image.FromStream(stream);
                }
                return result;
            }
        }

        /// <summary>
        /// Resources byte array returned as image
        /// </summary>
        private static Image ImageLoad
        {
            get
            {
                Image result;
                using (MemoryStream stream = new MemoryStream(Resources.imgLoad))
                {
                    result = Image.FromStream(stream);
                }
                return result;
            }
        }


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

            // define and position each textbox and label to suit its puropse
            SetupTextbox(out _txtSideA, out _lblSideA, this, TextboxType.SideA);
            SetupTextbox(out _txtAngleA, out _lblAngleA, this, TextboxType.AngleA);
            SetupTextbox(out _txtSideB, out _lblSideB, this, TextboxType.SideB);
            SetupTextbox(out _txtAngleB, out _lblAngleB, this, TextboxType.AngleB);
            SetupTextbox(out _txtSideC, out _lblSideC, this, TextboxType.SideC);
            SetupTextbox(out _txtAngleC, out _lblAngleC, this, TextboxType.AngleC);

            // set arrays of textboxes and labels once defined
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

            // define and position buttons and listbox
            SetupButton(out _btnSolve, ButtonNames.btnSolve, "Solve");
            SetupButton(out _btnClear, ButtonNames.btnClear, "Clear", _btnSolve);
            SetupButton(out _btnRotate, ButtonNames.btnRotate, "Rotate", _btnClear);
            SetupButton(out _btnSave, ButtonNames.btnSave, "Save", _btnRotate);
            SetupButton(out _btnLoad, ButtonNames.btnLoad, "Load", _btnSave);


            // define and position result list
            SetupListBox(out _listBox);

            // add to form
            Controls.Add(_btnSolve);
            Controls.Add(_btnClear);
            Controls.Add(_btnRotate);
            Controls.Add(_btnSave);
            Controls.Add(_btnLoad);

            Controls.Add(_listBox);

            // set up acceptbutton (return accept) and escape to clear
            AcceptButton = _btnSolve;
            CancelButton = _btnClear;

            // define opposing pairs by assigning textboxes
            _a = new(_txtAngleA, _txtSideA);
            _b = new(_txtAngleB, _txtSideB);
            _c = new(_txtAngleC, _txtSideC);

            // define triangle with links to opposing pairs / textboxes
            _triangle = new(_a, _b, _c);

            // define hatched workarea with triangle
            _workarea = new(this, _triangle);

            // display all items in list
            _listBox.Items.Clear();
            _listBox.Items.AddRange(_triangle.ResultList);

            // force OnPaint to redraw the triangle
            Invalidate(_workarea.Bounds);
        }

        /// <summary>
        /// Set up initial button
        /// </summary>
        /// <param name="button"></param>
        /// <param name="name"></param>
        /// <param name="text"></param>
        private void SetupButton(out Button button, string name, string text)
        {
            SetupButton(out button, name, text, null);
        }

        /// <summary>
        /// Set up additional button located beneath <paramref name="btnAbove"/>
        /// </summary>
        /// <param name="button"></param>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <param name="btnAbove"></param>
        private void SetupButton(out Button button, string name, string text, Button? btnAbove)
        {
            button = new Button()
            {
                Name = name,
                Text = text,
                Height = BTN.HEIGHT,
                Width = BTN.WIDTH,
                Left = FRM.BORDER,
                ImageAlign = ContentAlignment.MiddleLeft
            };

            if (btnAbove == null)
            {
                button.Top = FRM.BORDER;
            }
            else
            {
                button.Top = btnAbove.Top + btnAbove.Height + FRM.BORDER;
            }

            if (name == ButtonNames.btnSolve) { button.Image = ImageSolve; }
            if (name == ButtonNames.btnRotate) { button.Image = ImageRotate; }
            if (name == ButtonNames.btnClear) { button.Image = ImageClear; }
            if (name == ButtonNames.btnSave) { button.Image = ImageSave; }
            if (name == ButtonNames.btnLoad) { button.Image = ImageLoad; }

            button.Click += Button_Click;
        }

        /// <summary>
        /// Set up listbox for displaying output data for triangle and alternate triangle
        /// </summary>
        /// <param name="listbox"></param>
        private void SetupListBox(out ListBox listbox)
        {
            // retrieve font, change size and style
            Font font = new("Courier New", 8, FontStyle.Regular, GraphicsUnit.Point);
            listbox = new ListBox() { Width = BTN.WIDTH, Top = FRM.BORDER, Left = ClientRectangle.Width - BTN.WIDTH - FRM.BORDER, Font = font, IntegralHeight = false, TabStop = false };
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
            txtbox = new TextBox() { Name = name, Height = TXT.HEIGHT, Width = TXT.WIDTH, TextAlign = HorizontalAlignment.Right };

            // retrieve font, change size and style
            Font font = new(txtbox.Font.FontFamily.Name, 12, FontStyle.Bold, GraphicsUnit.Point);

            // define label and apply properties
            lbl = new Label() { Name = "lbl" + name, Text = caption, Font = font, Width = TXT.WIDTH, ForeColor = Color.SteelBlue };

            // set keypress and enter event handlers
            txtbox.KeyPress += TextBox_KeyPress;
            txtbox.Enter += Textbox_Enter;
            txtbox.Leave += Textbox_Leave;
            txtbox.KeyUp += TextBox_KeyUp;
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
                        txtbox.MaxLength = 10; // 9999999.99

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
                        txtbox.MaxLength = 10; // 9999999.99

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
                        txtbox.MaxLength = 10; // 9999999.99

                        // position label beside
                        lbl.Top = txtbox.Top;
                        lbl.Left = txtbox.Left - lbl.Width - FRM.BORDER;
                        lbl.Anchor = txtbox.Anchor;
                        lbl.TextAlign = ContentAlignment.MiddleRight;

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
                        lbl.TextAlign = ContentAlignment.MiddleRight;

                        break;
                    }
            }
        }

        // not in use
        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Get a text file in local user documents file named triggy.txt
        /// </summary>
        /// <param name="file">Defines file in user's documents folder and returns new <see cref="FileInfo"/> at that location.</param>
        /// <returns>Boolean value to determine if file exists at that location</returns>
        private static bool GetTempFile(out FileInfo file)
        {
            // get documents folder
            StringBuilder s = new StringBuilder(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            // add system path separator (/ or \)
            s.Append(Path.DirectorySeparatorChar);
            // add triggy
            s.Append(Application.ProductName);
            // add .txt
            s.Append(FILEEXT);

            // return new file as reference
            file = new FileInfo(s.ToString());

            // return whether the file exists
            return file.Exists;
        }

        /// <summary>
        /// Writes the contents of textbox data to text file
        /// </summary>
        /// <param name="file"></param>
        private void WriteTextBoxesToFile(FileInfo file)
        {
            Debug.WriteLine(file.FullName);

            // save to file
            using (StreamWriter s = new(file.FullName))
            {
                // loop through textboxes to store order and value
                foreach (TextBox t in _recentTextBoxes)
                {
                    // write name and value separated by a comma
                    s.WriteLine($"{t.Name},{t.Text}");
                }
            }

            // refresh variable if file was created
            file.Refresh();

            // if file was successfully written
            if (file.Exists)
            {
                // start new process
                using (Process p = new())
                {
                    // set explorer as process name so it opens default app for filetype
                    p.StartInfo.FileName = "explorer.exe";
                    // specify filename as the only argument
                    p.StartInfo.Arguments = file.FullName;
                    // start process (open triggy.txt file in notepad)
                    p.Start();
                }
            }
        }

        /// <summary>
        /// Reads the contents of text file data into the textboxes
        /// </summary>
        /// <param name="file"></param>
        private void ReadTextBoxesFromFile(FileInfo file)
        {
            // List to store lines from the file
            List<string> lines = new();

            // Read all lines from the file
            using (StreamReader s = new(file.FullName))
            {
                // read each line in the file
                while (!s.EndOfStream)
                {
                    // read into nullable string
                    string? line = s.ReadLine();
                    // ensure line is not empty (or just spaces)
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        // only include line if it contains a comma char ()
                        if (line.Contains(','))
                        {
                            // Add the line to the list
                            lines.Add(line);
                        }
                    }
                }
            }

            // ensure file was found and contains valid items
            if (lines.Count > 0)
            {
                // Regex pattern to match two groups separated by a single comma
                // specifically, text to the left of comma, number (possible decimal place) to right of comma
                Regex rex = new(@"^(.*?),([0-9]*\.?[0-9]+)$");

                // store textboxes into temp array
                TextBox[] temp = _recentTextBoxes.ToArray();

                // clear list
                _recentTextBoxes.Clear();

                // loop through each line in text file
                foreach (string eachLine in lines)
                {
                    // process the line with the regex
                    // much more efficient and effective than using `string.split`
                    Match match = rex.Match(eachLine);

                    // ensure success AND more than two match groups found
                    if (match.Success && match.Groups.Count >= 2)
                    {
                        // index 0 is always the full string 
                        string txtName = match.Groups[1].Value;  // index 1 is the first capture group (anything)
                        string txtValue = match.Groups[2].Value; // index 2 is the second capture group (numbers only)

                        // Example: Debug output for matched groups
                        Debug.WriteLine($"Group 1: {txtName}, Group 2: {txtValue}");

                        // move temp items to collection in correct order and set value
                        foreach (TextBox eachTextbox in temp)
                        {
                            // if name of textbox matches
                            if (eachTextbox.Name == txtName)
                            {
                                // set value to textbox
                                eachTextbox.Text = txtValue;

                                // only add back textbox list once
                                // - this should only NOT add it, if the text file is
                                //   modified to include one textbox multiple times
                                if (!_recentTextBoxes.Contains(eachTextbox))
                                {
                                    // add textbox back to original collection
                                    _recentTextBoxes.Add(eachTextbox);

                                    // exit this foreach loop and continue next eachLine
                                    break;
                                }
                            }
                        }
                    }
                }

                // add any textboxes that were not added
                // consider each textbox in the temp collection
                foreach (TextBox eachTemp in temp)
                {
                    // found flag
                    bool found = false;

                    // check if each textboxe as re-added
                    // consider each textbox in the main collection
                    foreach (TextBox eachTextbox in _recentTextBoxes)
                    {
                        // if the names match
                        if (eachTextbox.Name == eachTemp.Name)
                        {
                            // flag as found
                            found = true;

                            // exit loop (no need to check each other item once found)
                            break;
                        }
                    }

                    // after the loop we know if the textbox was added to the main collection
                    if (!found)
                    {
                        Debug.WriteLine($"Missing textbox added {eachTemp.Name}");

                        // add missing textbox if not already
                        _recentTextBoxes.Add(eachTemp);

                        // THIS ENSURES ALL SIX TEXTBOXES ARE ALWAYS IN THE LIST
                    }
                }
            }
        }

        /// <summary>
        /// handle button click to perform specific actions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object? sender, EventArgs e)
        {
            // rotate all text values before recalculating
            if (sender == _btnRotate)
            {
                // store A and a
                string A = _txtAngleA.Text;
                string a = _txtSideA.Text;

                // rotate counter-clockwise
                _txtAngleA.Text = _txtAngleB.Text;
                _txtAngleB.Text = _txtAngleC.Text;
                _txtAngleC.Text = A;
                _txtSideA.Text = _txtSideB.Text;
                _txtSideB.Text = _txtSideC.Text;
                _txtSideC.Text = a;

                // continue to solve the new triangle
            }

            if (sender == _btnLoad)
            {
                // create local reference variable to fileinfo
                if (GetTempFile(out FileInfo f))
                {
                    // read text file contents and add update textbox collection
                    ReadTextBoxesFromFile(f);

                    // continue to solve the loaded triangle
                }
                else
                {
                    // warn file does not yet exist
                    MessageBox.Show(this, "Triangle data has not yet been saved.", "Triggy", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    // don't continue further
                    return;
                }
            }

            if (sender == _btnSave)
            {
                // create local reference variable to fileinfo
                GetTempFile(out FileInfo f);

                // shouldn't be null but just in case
                if (f != null)
                {
                    // write textbox contents to text file
                    WriteTextBoxesToFile(f);
                }

                // don't continue further
                return;
            }
            else if (sender == _btnClear)
            {
                // clear all triangle data
                _triangle.Clear();
            }
            else
            {
                // clear the oldest three textboxes (grey text)
                // this is to prevent confusing algorithms in the event of conflicting superfluous data
                // the grey text in the UI implies these values may change
                for (int i = 3; i < _recentTextBoxes.Count; i++)
                {
                    // clear each textbox for index 3, 4, 5
                    _recentTextBoxes[i].Text = string.Empty;
                }

                // define preliminary data using current textbox values
                _triangle.GetTextBoxValues();

                // only solve if sufficient data is provided
                if (_triangle.IsSufficient)
                {
                    // list to collect error messages
                    // replaced with HashSet to exclude repeats of the same message
                    HashSet<string> resultMessages = new();

                    if (_triangle.CheckAngles(ref resultMessages))
                    {
                        // try to solve and store result
                        if (_triangle.SolveSSA(ref resultMessages))
                        {
                            Debug.WriteLine(" Solved Side Side Angle");
                        }
                        else if (_triangle.SolveASA(ref resultMessages))
                        {
                            Debug.WriteLine(" Solved Angle Side Angle");
                        }
                        else if (_triangle.SolveSAS(ref resultMessages))
                        {
                            Debug.WriteLine(" Solved Side Angle Side");
                        }
                        else if (_triangle.SolveSSS(ref resultMessages))
                        {
                            Debug.WriteLine(" Solved Side Side Side");
                        }
                    }

                    // warn if issues were found with input data
                    if (resultMessages.Count > 0)
                    {
                        // show one messagebox with each error on a new line
                        MessageBox.Show(this, string.Join(Environment.NewLine, resultMessages), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            //
            // this should still run after solve, clear, and a successful load //
            //

            // display triangle results in all textboxes
            _triangle.SetTextBoxValues();

            // create carbon copy for workarea scaling
            _workarea.CarbonCopy();

            // force OnPaint to redraw the triangle
            Invalidate(_workarea.Bounds);

            // update all results in list
            _listBox.Items.Clear();
            _listBox.Items.AddRange(_triangle.ResultList);

            // focus on first textbox
            _recentTextBoxes[0].Select();

            // display name
            NameTriangle();
        }

        /// <summary>
        /// Determines the type of triangle based on equality of sides and angles
        /// <para>Updates window text to show name of triangle</para>
        /// </summary>
        private void NameTriangle()
        {
            // more efficient method of combining string values
            StringBuilder stringBuilder = new StringBuilder("Triggy the Love Triangle");

            if (_triangle.IsComplete)
            {
                stringBuilder.Append(" (");

                // create set using hash function, implies no repeated values
                HashSet<Length> sides = new() { _triangle.A.Side, _triangle.B.Side, _triangle.C.Side };
                HashSet<Angle> angles = new() { _triangle.A.Vertex, _triangle.B.Vertex, _triangle.C.Vertex };

                // describe angles

                bool right = false;
                bool obtuse = false;

                foreach (Angle angle in angles)
                {
                    // using operators here! 
                    if (angle > 90) { obtuse = true; break; }
                    if (angle == 90) { right = true; break; }
                }

                if (right) { stringBuilder.Append("Right"); }
                else if (obtuse) { stringBuilder.Append("Obtuse"); }
                else { stringBuilder.Append("Acute"); }

                stringBuilder.Append(", ");

                // describe sides

                switch (sides.Count)
                {
                    case 1:
                        // all the same
                        stringBuilder.Append("Equalateral");
                        break;
                    case 2:
                        // two the same
                        stringBuilder.Append("Isoceles");
                        break;
                    default:
                        // three different
                        stringBuilder.Append("Scalene");
                        break;
                }

                // append single character
                stringBuilder.Append(')');
            }

            // update form caption to show triangle name
            Text = stringBuilder.ToString();
        }

        /// <summary>
        /// Arrange textboxes in list and colour text black or grey accordingly
        /// <para>Should be called after a textbox gains or loses focus and after each keypress
        /// <br>Necessary to correctly determine which three are input and which three are output</br></para>
        /// </summary>
        /// <param name="sender"></param>
        private void TextBox_EnterLeave(object sender)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox == null) return;

            Debug.WriteLine($"Textbox {textBox.Name}: '{textBox.Text}'");

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

                    // determine if angle textbox is > 180°
                    if ((_recentTextBoxes[i].Equals(_txtAngleA) ||
                        _recentTextBoxes[i].Equals(_txtAngleB) ||
                        _recentTextBoxes[i].Equals(_txtAngleC)) &&
                        _recentTextBoxes[i].ToDouble() >= 180)
                    {
                        // warning back colour
                        _recentTextBoxes[i].BackColor = Color.MistyRose;
                    }
                    else
                    {
                        // normal back colour
                        _recentTextBoxes[i].BackColor = SystemColors.Window;
                    }
                }
                else
                {
                    // grey for secondary 3
                    _recentTextBoxes[i].ForeColor = Color.DarkGray;

                    // normal back colour
                    _recentTextBoxes[i].BackColor = SystemColors.Window;
                }
            }
        }

        // handle textbox key up events to track last 3 focussed textboxes
        private void TextBox_KeyUp(object? sender, KeyEventArgs e)
        {
            if (sender is TextBox)
            {
                // set textbox to top of list and colour text black
                TextBox_EnterLeave(sender);
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

        // handle textbox leave events to track last 3 focussed textboxes
        private void Textbox_Leave(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                // set setbox to bottom of list and colour text grey
                TextBox_EnterLeave(sender);
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
                    return; // Allow the input
                }

                // Allow a single decimal point
                if (e.KeyChar == '.')
                {
                    // if no decimal is added yet, or the entiore contents is selected
                    if (!textBox.Text.Contains('.') || (textBox.SelectionLength == textBox.TextLength))
                    {
                        return; // Allow the input
                    }

                    // I could also go one step further and determine if the
                    // selected text contains a '.' but you get the idea
                }

                // If the input is not valid, suppress it
                e.Handled = true;
            }
        }

        /// <summary>
        /// Override form's own OnResize to reposition textboxes and redraw triangle (OnPaint)
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
        /// Override form's own OnPaint to draw the triangle
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_workarea != null)
            {
                Graphics g = e.Graphics;

                // using statements for classes that implement the iDisposable interface to ensure correct garbage collection

                // fill display bounds with light purple squares
                using (Brush brush = new HatchBrush(HatchStyle.DottedGrid, Color.Lavender, Color.GhostWhite))
                {
                    g.FillRectangle(brush, _workarea.Bounds);
                }

                // outline display bounds
                using (Pen pen = new(Color.MediumPurple, 1))
                {
                    g.DrawRectangle(pen, _workarea.Bounds);
                }

                // draw alternate triangle first so outline doesn't block main triangle
                if (!_workarea.AlternatePoints.IsEmpty)
                {
                    // fill with transparent *back*colour
                    using (Brush brush = new HatchBrush(HatchStyle.Percent30, Color.LightSteelBlue, Color.Transparent))
                    {
                        g.FillPolygon(brush, _workarea.AlternatePoints.ToArray());
                    }

                    // draw outline for alternate triangle
                    using (Pen pen = new(Color.LightSteelBlue, 1))
                    {
                        // Draw triangle
                        g.DrawPolygon(pen, _workarea.AlternatePoints.ToArray());
                    }
                }

                // draw main triangle over alternate triangle
                if (!_workarea.PrimaryPoints.IsEmpty)
                {
                    // fill with transparent *fore*colour (inverse of alternate triangle so both are visible when overlapping)
                    using (Brush brush = new HatchBrush(HatchStyle.Percent40, Color.Transparent, Color.SteelBlue))
                    {
                        g.FillPolygon(brush, _workarea.PrimaryPoints.ToArray());
                    }

                    // draw outline for primary triangle
                    using (Pen pen = new(Color.MidnightBlue, 2))
                    {
                        // Draw triangle
                        g.DrawPolygon(pen, _workarea.PrimaryPoints.ToArray());
                    }
                }

            }
        }

    }
}
