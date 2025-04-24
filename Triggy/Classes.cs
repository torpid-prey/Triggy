using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using triggy;

namespace triggy
{

    enum PairStatus
    {
        NEITHER = 0,
        SIDE = 1,
        ANGLE = 2,
        BOTH = 3
    };

    public struct FRM
    {
        public const int HEIGHT = 600;          // height
        public const int WIDTH = 800;           // width
        public const int BORDER = 10;           // border
        public const int BORDER2 = 2 * BORDER;  // double border size
        public const int INSET = 30;            // triangle inset from workarea on one edge
        public const int INSET2 = 2 * INSET;    // triangle inset from workarea on both edges
    }

    public struct TXT
    {
        public const int HEIGHT = 29; // textbox height
        public const int WIDTH = 100; // textbox width
    }

    public struct BTN
    {
        public const int HEIGHT = 40; // button height
        public const int WIDTH = 100; // button width
    }



    public static class Extensions
    {

        /// <summary>
        /// Returns textbox value as double
        /// </summary>
        /// <param name="txtbox"></param>
        /// <returns></returns>
        public static double ToDouble(this TextBox txtbox)
        {
            double result = 0;
            if (txtbox != null)
            {
                if (!double.TryParse(txtbox.Text, out result))
                {
                    return 0;
                }
            }
            return result;
        }

        /// <summary>
        /// Determines if the textbox contains no value
        /// </summary>
        /// <param name="txtbox"></param>
        /// <returns></returns>
        public static bool Empty(this TextBox txtbox)
        {
            if (txtbox != null)
            {
                return string.IsNullOrEmpty(txtbox.Text);
            }
            return true;
        }
    } // extensions


    /// <summary>
    /// Angle that calculates both degrees and radians
    /// </summary>
    internal class Angle
    {
        /// <summary>
        /// local member that stores degrees value
        /// </summary>
        private double _deg;

        /// <summary>
        /// local member that stores radians
        /// </summary>
        private double _rad;

        /// <summary>
        /// Set Degrees and convert to Radians
        /// </summary>
        internal double Degrees
        {
            get { return _deg; }
            set
            {
                // store degrees
                _deg = value;

                // convert to radians
                _rad = _deg * (Math.PI / 180);
            }
        }


        /// <summary>
        /// Set Radians and convert to Degrees
        /// </summary>
        internal double Radians
        {
            get { return _rad; }
            set
            {
                // store radians
                _rad = value;

                // convert to degrees
                _deg = _rad * (180 / Math.PI);
            }
        }

        /// <summary>
        /// Determines if <see cref="Angle"/> = 0
        /// </summary>
        internal bool Empty
        {
            get
            {
                return _deg == 0;
            }
        }

        /// <summary>
        /// Declare empty angle
        /// </summary>
        internal Angle() { }

        /// <summary>
        /// Declare angle using degress
        /// </summary>
        /// <param name="deg"></param>
        internal Angle(double deg)
        {
            // store angle and convert to radians
            Degrees = deg;
        }

        /// <summary>
        /// Deduce the remaining angle by subtracting the two angles from 180 degrees
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="resultMessages"></param>
        /// <returns></returns>
        internal bool Deduce(OpposingPair A, OpposingPair B, ref List<string> resultMessages)
        {
            // ensure all angles combined are less than 180 degrees
            if (Degrees < 180 && (A.Vertex.Degrees + B.Vertex.Degrees) < 180)
            {
                // calculate the angle
                Degrees = 180 - A.Vertex.Degrees - B.Vertex.Degrees;

                // return success
                return true;
            }
            else
            {
                // record error
                resultMessages.Add("Two provided angles must not be greater than 180 degrees.");
            }
            // return incomplete
            return false;
        }

        /// <summary>
        /// Converts numeric value of this instance to its equivalent string, to two decimal places
        /// </summary>
        /// <returns>The string value of this instance.</returns>
        public override string ToString()
        {
            return _deg.ToString("0.00");
        }
    } // Angle


    /// <summary>
    /// Side that calculates itself raised to power of 2 if required
    /// <br>Can also include method to scale when required</br>
    /// </summary>
    internal class Length
    {
        /// <summary>
        /// Gets or sets the side Value
        /// </summary>
        internal double Value { get; set; }

        /// <summary>
        /// Returns Value to the power of 2
        /// </summary>
        internal double Squared
        {
            get
            {
                // return value to power of 2
                return Math.Pow(Value, 2);
            }
        }

        /// <summary>
        /// Define empty length
        /// </summary>
        internal Length()
        {
            Value = 0;
        }

        /// <summary>
        /// Define side length
        /// </summary>
        /// <param name="val"></param>
        internal Length(double val)
        {
            Value = val;
        }

        /// <summary>
        /// Determine if the side value is empty
        /// </summary>
        internal bool Empty
        {
            get
            {
                return Value == 0;
            }
        }

        /// <summary>
        /// Clears the current side value
        /// </summary>
        internal void Clear()
        {
            Value = 0;
        }

        /// <summary>
        /// Converts numeric value of this instance to its equivalent string, to two decimal places
        /// </summary>
        /// <returns>The string value of this instance.</returns>
        public override string ToString()
        {
            return Value.ToString("0.00");
        }

    } // Length






    /// <summary>
    /// Combined <see cref="Angle"/> and opposite side <see cref="Length"/>
    /// </summary>
    internal class OpposingPair : IDisposable
    {
        private bool _disposed = false;

        /// <summary>
        /// Clear references to TextBox objects
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                // Clear references to TextBox objects
                if (txtVertex != null) txtVertex.Text = string.Empty;
                if (txtSide != null) txtSide.Text = string.Empty;

                // Dispose of the triangle solver
                _disposed = true;
            }

        }


        /// <summary>
        /// Vertex opposite to the side
        /// </summary>
        internal Angle Vertex { get; set; }

        /// <summary>
        /// Side opposite to the vertex
        /// </summary>
        internal Length Side { get; set; }

        // textbox references for quick update
        private readonly TextBox? txtVertex = null;
        private readonly TextBox? txtSide = null;

        /// <summary>
        /// Determine if this instance contains Side, Angle, Both or Neither
        /// </summary>
        internal PairStatus GetStatus
        {
            get
            {
                int result = (int)PairStatus.NEITHER;
                if (!Side.Empty) { result += (int)PairStatus.SIDE; }
                if (!Vertex.Empty) { result += (int)PairStatus.ANGLE; }
                return (PairStatus)result;
            }
        }


        /// <summary>
        /// Determines if the side is defined
        /// </summary>
        /// <returns></returns>
        internal bool HasSide()
        {
            return !Side.Empty;
        }

        /// <summary>
        /// Determines if the side is defined
        /// </summary>
        /// <param name="accum">Accumulate number of sides</param>
        /// <returns></returns>
        internal bool HasSide(ref int accum)
        {
            if (!Side.Empty) { accum++; }
            return !Side.Empty;
        }

        /// <summary>
        /// Determines if the angle is defined
        /// </summary>
        /// <returns></returns>
        internal bool HasAngle()
        {
            return !Vertex.Empty;
        }

        /// <summary>
        /// Determines if the angle is defined
        /// </summary>
        /// <param name="accum">Accumulate number of angles</param>
        /// <returns></returns>
        internal bool HasAngle(ref int accum)
        {
            if (!Vertex.Empty) { accum++; }
            return !Vertex.Empty;
        }

        /// <summary>
        /// Define empty <see cref="OpposingPair"/>
        /// <br>Stores the Vertex and Side that are opposite one another</br>
        /// </summary>
        internal OpposingPair()
        {
            // initialise the private fields not the properties
            Vertex = new Angle();
            Side = new Length();
        }

        /// <summary>
        /// Define new <see cref="OpposingPair"/> using values from <see cref="TextBox"/> objects
        /// </summary>
        /// <param name="vertex">Define the vertex angle in degrees</param>
        /// <param name="side">Define the side length</param>
        internal OpposingPair(TextBox vertex, TextBox side)
        {
            // use extension methods to convert textbox values to double
            Vertex = new Angle(vertex.ToDouble());
            Side = new Length(side.ToDouble());

            // store textboxes if specified
            txtVertex = vertex;
            txtSide = side;
        }

        /// <summary>
        /// Define new <see cref="OpposingPair"/> from an existing instance
        /// </summary>
        /// <param name="existing"></param>
        internal OpposingPair(OpposingPair existing)
        {
            // initialise the private fields not the properties
            Vertex = new Angle(existing.Vertex.Degrees);
            Side = new Length(existing.Side.Value);
        }

        /// <summary>
        /// Checks side lengths against those in other <see cref="OpposingPair"/>(s)
        /// </summary>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="returnMessage"></param>
        /// <returns></returns>
        internal bool Check(OpposingPair B, OpposingPair C, ref List<string> returnMessage)
        {
            if (Side.Value > B.Side.Value + C.Side.Value)
            {
                returnMessage.Add("One side cannot be longer than the other two sides.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the remaining vertex angle using other vertex angles
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        internal bool GetAngle(Angle A, Angle B, ref List<string> resultMessages)
        {
            if (!A.Empty && !B.Empty)
            {
                if (A.Degrees + B.Degrees < 180)
                {
                    // calculate the angle
                    Vertex = new Angle(180 - A.Degrees - B.Degrees);

                    // return success
                    return true;
                }
                else
                {
                    // record error
                    resultMessages.Add("Two provided angles must not be greater than 180 degrees.");
                }
            }

            // return incomplete
            return false;
        }

        /// <summary>
        /// Get side value using complete <see cref="OpposingPair"/>
        /// <br>Requires vertex angle to be defined in the current <see cref="OpposingPair"/></br>
        /// </summary>
        /// <param name="C"></param>
        internal bool GetSide(OpposingPair B, ref List<string> resultMessages)
        {
            // may be other checks to ensure pair is valid before proceeding

            if (!Vertex.Empty && B.GetStatus == PairStatus.BOTH)
            {
                if (Vertex.Degrees < 180)
                {
                    // calculate own side using another complete opposing pair and own angle
                    Side.Value = (B.Side.Value * Math.Sin(Vertex.Radians)) / Math.Sin(B.Vertex.Radians);

                    return true;
                }
                else
                {
                    // record error
                    resultMessages.Add("Provided angle must not be greater than 180 degrees.");
                }
            }

            return false;
        }

        /// <summary>
        /// Get current side value using <see cref="Length"/> from two adjacent sides.
        /// <br>Requires vertex angle to be defined in the current <see cref="OpposingPair"/></br>
        /// </summary>
        /// <param name="B"></param>
        /// <param name="C"></param>
        internal bool GetSide(Length B, Length C, ref List<string> resultMessages)
        {
            // may be  other checks to ensure pair is valid before proceeding

            if (!Vertex.Empty && !B.Empty && !C.Empty)
            {
                if (Vertex.Degrees < 180)
                {
                    // calculate own side using adjacent sides and own angle
                    Side.Value = Math.Sqrt(B.Squared + C.Squared - 2 * B.Value * C.Value * Math.Cos(Vertex.Radians));

                    return true;
                }
                else
                {
                    // record error
                    resultMessages.Add("Provided angle must not be greater than 180 degrees.");
                }
            }

            return false;
        }

        /// <summary>
        /// Get current vertex angle using complete <see cref="OpposingPair"/>
        /// <br>Requires side value to be defined in the current <see cref="OpposingPair"/></br>
        /// </summary>
        internal bool GetAngle(OpposingPair B, ref List<string> resultMessages)
        {
            if (!Side.Empty && B.GetStatus == PairStatus.BOTH)
            {
                double _asin = (Side.Value * Math.Sin(B.Vertex.Radians)) / B.Side.Value;

                if (_asin >= -1 && _asin <= 1)
                {
                    // calculate result in radians
                    Vertex.Radians = Math.Asin(_asin);

                    return true;
                }
                else
                {
                    // determine what causes this error
                    resultMessages.Add("Value for angle is too large. No intersection could be found.");
                }
            }
            return false;
        }

        /// <summary>
        /// Get current vertex angle using <see cref="Length"/> from two adjacent sides.
        /// <br>Requires side value to be defined in the current <see cref="OpposingPair"/></br>
        /// </summary>
        /// <param name="B"></param>
        /// <param name="C"></param>
        internal bool GetAngle(Length B, Length C, ref List<string> resultMessages)
        {
            // determine if there are other checks to ensure this will work

            if (!Side.Empty)
            {
                try
                {
                    // calculate result in radians
                    Vertex.Radians = Math.Acos((B.Squared + C.Squared - Side.Squared) / (2 * B.Value * C.Value));

                    return true;

                }
                catch (Exception ex)
                {
                    // provide error 
                    resultMessages.Add(ex.Message);
                }
            }

            return false;
        }

        /// <summary>
        /// Display the opposing pair values in the provided textboxes
        /// </summary>
        internal void SetTextBoxValues()
        {
            if (txtVertex != null)
            {
                // clear if zero, otherwise set value
                txtVertex.Text = Vertex.Degrees == 0 ? string.Empty : Vertex.Degrees.ToString("0.00");
            }

            if (txtSide != null)
            {
                // clear if zero, otherwise set value
                txtSide.Text = Side.Value == 0 ? string.Empty : Side.Value.ToString("0.00");
            }

        }

        /// <summary>
        /// Read the provided textbox values into the opposing pairs
        /// </summary>
        internal void GetTextBoxValues()
        {
            if (txtVertex != null) Vertex.Degrees = txtVertex.ToDouble();
            if (txtSide != null) Side.Value = txtSide.ToDouble();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return ("Side: " + Side.ToString() + " Angle: " + Vertex.ToString());
        }

    } // Opposing Pair


    /// <summary>
    /// Combine 3 <see cref="OpposingPair"/> objects to solve for a complete trianlge
    /// <br>(3 <see cref="Length"/>s and 3 <see cref="Angle"/>s)</br>
    /// </summary>
    internal class TriangleSolver : IDisposable
    {
        private bool _disposed = false;

        /// <summary>
        /// Dispose of the triangle solver
        /// </summary>
        public void Dispose()
        {

            if (!_disposed)
            {
                // Clear references to TextBox objects
                A.Dispose();
                B.Dispose();
                C.Dispose();
                AltTriangle?.Dispose();

                // Dispose of the triangle solver
                _disposed = true;
            }

        }

        // Main triangle
        private OpposingPair _a;
        private OpposingPair _b;
        private OpposingPair _c;

        // Fields used to store original sides for dynamic scaling
        private double _origSideA;
        private double _origSideB;
        private double _origSideC;

        /// <summary>
        /// Store original sides if side values are not empty
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        internal void SetOriginalSides()
        {
            // store original side lengths for dynamic scaling
            if (_a.HasSide()) { _origSideA = _a.Side.Value; }
            if (_b.HasSide()) { _origSideB = _b.Side.Value; }
            if (_c.HasSide()) { _origSideC = _c.Side.Value; }
        }

        /// <summary>
        /// Returns the bottom right angle and opposing left side
        /// </summary>
        internal ref OpposingPair A
        {
            get { return ref _a; }
        }

        /// <summary>
        /// Returns the bottom left angle and opposing right side
        /// </summary>
        internal ref OpposingPair B
        {
            get { return ref _b; }
        }

        /// <summary>
        /// Returns the top angle and opposing bottom side
        /// </summary>
        internal ref OpposingPair C
        {
            get { return ref _c; }
        }

        /// <summary>
        /// Returns instance of alternate triangle used to determine height and complete width
        /// </summary>
        internal TriangleSolver? AltTriangle { get; private set; }



        /// <summary>
        /// Define new <see cref="TriangleSolver"/> from known <see cref="OpposingPair"/> objects
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        internal TriangleSolver(OpposingPair a, OpposingPair b, OpposingPair c)
        {
            _a = a;
            _b = b;
            _c = c;
        }


        /// <summary>
        /// Define new <see cref="TriangleSolver"/> from existing <see cref="TriangleSolver"/> object
        /// </summary>
        /// <param name="triangle">Current instance of <see cref="TriangleSolver"/> to copy</param>
        internal TriangleSolver(TriangleSolver triangle)
        {
            // need to create new instances of opposing pair objects otherwise
            // updates will still affect the existing instances of opposing pairs
            _a = new OpposingPair(triangle.A);
            _b = new OpposingPair(triangle.B);
            _c = new OpposingPair(triangle.C);

            // store original side lengths for dynamic scaling
            SetOriginalSides();

            if (triangle.AltTriangle != null)
            {
                // copy alt triangle if available
                AltTriangle = new TriangleSolver(triangle.AltTriangle);

                //// store original side lengths for dynamic scaling
                //AltTriangle.SetOriginalSides();

            }
        }

        /// <summary>
        /// Read the provided textbox values into the opposing pairs
        /// </summary>
        internal void GetTextBoxValues()
        {
            _a.GetTextBoxValues();
            _b.GetTextBoxValues();
            _c.GetTextBoxValues();
        }

        /// <summary>
        /// Display the opposing pairs in the provided textboxes
        /// <para>Also updates the original side values used for scaling</para>
        /// </summary>
        internal void SetTextBoxValues()
        {
            _a.SetTextBoxValues();
            _b.SetTextBoxValues();
            _c.SetTextBoxValues();

            // store original side lengths for dynamic scaling
            SetOriginalSides();
        }

        internal string[] GetResultList()
        {
            List<string> result =
            [
                $"A° {_a.Vertex.Degrees:0.00}",
                $"a  {_a.Side.Value:0.00}",
                $"B° {_b.Vertex.Degrees:0.00}",
                $"b  {_b.Side.Value:0.00}",
                $"C° {_c.Vertex.Degrees:0.00}",
                $"c  {_c.Side.Value:0.00}",
            ];

            if (AltTriangle != null)
            {
                result.Add("");
                result.Add($"D° {AltTriangle._a.Vertex.Degrees:0.00}");
                result.Add($"d  {AltTriangle._a.Side.Value:0.00}");
                result.Add($"E° {AltTriangle._b.Vertex.Degrees:0.00}");
                result.Add($"e  {AltTriangle._b.Side.Value:0.00}");
                result.Add($"F° {AltTriangle._c.Vertex.Degrees:0.00}");
                result.Add($"f  {AltTriangle._c.Side.Value:0.00}");
            }

            return result.ToArray();
        }

        /// <summary>
        /// Scale the side lengths of the triangle by a given factor
        /// <br>Also scales AltTriangle</br>
        /// </summary>
        /// <param name="factor"></param>
        internal void Scale(double factor)
        {
            // Scale main triangle
            _a.Side.Value = _origSideA * factor;
            _b.Side.Value = _origSideB * factor;
            _c.Side.Value = _origSideC * factor;

            // Scale alternate triangle if it exists
            AltTriangle?.Scale(factor);
        }

        /// <summary>
        /// Solve triangle using any two angles and one side
        /// </summary>
        /// <param name="resultMessages"></param>
        /// <returns></returns>
        internal bool SolveASA(ref List<string> resultMessages)
        {
            // if one side and any two angles are given

            // first complete angles if possible
            if (CompleteAngles(ref resultMessages))
            {
                // quick check if all sides are empty and assign one
                if (_a.Side.Empty && _b.Side.Empty && _c.Side.Empty)
                {
                    // assign one side, value does not matter
                    _c.Side.Value = 100;
                }

                // determine which OpposingPair is completed
                if (_a.GetStatus == PairStatus.BOTH)
                {
                    // if A is complete, then use A to complete B and C
                    if (_b.GetSide(_a, ref resultMessages) && _c.GetSide(_a, ref resultMessages))
                    {
                        CalculateAltTriangle(ref resultMessages);
                        return true;
                    }
                }
                else if (_b.GetStatus == PairStatus.BOTH)
                {
                    // if B is complete, then use B to complete A and C
                    if (_a.GetSide(_b, ref resultMessages) && _c.GetSide(_b, ref resultMessages))
                    {
                        CalculateAltTriangle(ref resultMessages);
                        return true;
                    }
                }
                else if (_c.GetStatus == PairStatus.BOTH)
                {
                    // if C is complete, then use C to complete A and B
                    if (_a.GetSide(_c, ref resultMessages) && _b.GetSide(_c, ref resultMessages))
                    {
                        CalculateAltTriangle(ref resultMessages);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Solve triangle using three sides
        /// </summary>
        /// <param name="resultMessages"></param>
        /// <returns></returns>
        internal bool SolveSSS(ref List<string> resultMessages)
        {
            if (!A.Side.Empty && !B.Side.Empty && !C.Side.Empty)
            {
                if (A.Check(B, C, ref resultMessages) &&
                    B.Check(A, C, ref resultMessages) &&
                    C.Check(A, B, ref resultMessages))
                {
                    if (A.GetAngle(B.Side, C.Side, ref resultMessages) &&
                        B.GetAngle(A.Side, C.Side, ref resultMessages) &&
                        C.GetAngle(A.Side, B.Side, ref resultMessages))
                    {
                        CalculateAltTriangle(ref resultMessages);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Solve triangle using one angle and two sides, when angle <u>is</u> between two adjacent sides
        /// </summary>
        /// <param name="resultMessages"></param>
        /// <returns></returns>
        internal bool SolveSAS(ref List<string> resultMessages)
        {
            if (!A.Side.Empty && !B.Vertex.Empty && !C.Side.Empty)
            {
                if (SolveSAS(ref A, ref B, ref C, ref resultMessages))
                {
                    CalculateAltTriangle(ref resultMessages);
                    return true;
                }
            }
            else if (!A.Side.Empty && !C.Vertex.Empty && !B.Side.Empty)
            {
                if (SolveSAS(ref A, ref C, ref B, ref resultMessages))
                {
                    CalculateAltTriangle(ref resultMessages);
                    return true;
                }
            }
            else if (!B.Side.Empty && !A.Vertex.Empty && !C.Side.Empty)
            {
                if (SolveSAS(ref B, ref A, ref C, ref resultMessages))
                {
                    CalculateAltTriangle(ref resultMessages);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Solve trianlge using one angle and two sides, when angle <u>is</u> between two adjacent sides
        /// </summary>
        /// <param name="sidePair1"></param>
        /// <param name="anglePair"></param>
        /// <param name="sidePair2"></param>
        /// <param name="resultMessages"></param>
        /// <returns></returns>
        static private bool SolveSAS(ref OpposingPair sidePair1, ref OpposingPair anglePair, ref OpposingPair sidePair2, ref List<string> resultMessages)
        {
            bool a, b, c;

            // first get the side opposite the angle
            b = anglePair.GetSide(sidePair1.Side, sidePair2.Side, ref resultMessages);

            // depends which side is smaller
            if (sidePair1.Side.Value <= sidePair2.Side.Value)
            {
                // complete angle pair 1
                a = sidePair1.GetAngle(anglePair, ref resultMessages);

                // complete remaining angle
                c = sidePair2.Vertex.Deduce(anglePair, sidePair1, ref resultMessages);
            }
            else
            {
                // complete angle pair 2
                a = sidePair2.GetAngle(anglePair, ref resultMessages);

                // complete remaining angle
                c = sidePair1.Vertex.Deduce(anglePair, sidePair2, ref resultMessages);
            }

            return (a && b && c);
        }


        /// <summary>
        /// Solve triangle using one angle and two sides, when angle <u>is not</u> between two adjacent sides
        /// </summary>
        /// <param name="resultMessages"></param>
        /// <returns></returns>
        internal bool SolveSSA(ref List<string> resultMessages)
        {
            // if two sides and one angle are given, when the angle is not between adjacent sides

            OpposingPair? _complete = null;
            OpposingPair? _incomplete = null;
            OpposingPair? _empty = null;

            // determine which opposing pair is complete
            if (A.GetStatus == PairStatus.BOTH)
            {
                _complete = A;

                // if B side is defined, then use A to complete B
                if (!B.Side.Empty)
                {
                    _incomplete = B;
                    _empty = C;
                }
                // if C side is defined, then use C to complete B
                else if (!C.Side.Empty)
                {
                    _incomplete = C;
                    _empty = B;
                }
            }
            else if (B.GetStatus == PairStatus.BOTH)
            {
                _complete = B;

                // if A side is defined, then use B to complete A
                if (!A.Side.Empty)
                {
                    _incomplete = A;
                    _empty = C;
                }
                // if C side is defined, then use B to complete C
                else if (!C.Side.Empty)
                {
                    _incomplete = C;
                    _empty = A;
                }
            }
            else if (C.GetStatus == PairStatus.BOTH)
            {
                _complete = C;

                // if A side is defined, then use C to complete A
                if (!A.Side.Empty)
                {
                    _incomplete = A;
                    _empty = B;
                }
                // if B side is defined, then use C to complete B
                if (!B.Side.Empty)
                {
                    _incomplete = B;
                    _empty = A;
                }
            }

            if (_complete != null && _incomplete != null && _empty != null)
            {
                if (SolveSSA(ref _complete, ref _incomplete, ref _empty, ref resultMessages))
                {
                    CalculateAltTriangle(ref resultMessages);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Static solver which depends on relationship between <see cref="OpposingPair"/> instances
        /// </summary>
        /// <param name="complete">Provide <see cref="OpposingPair"/> with both the Vertex and the Side defined</param>
        /// <param name="incomplete">Provide the <see cref="OpposingPair"/> with only a Side defined</param>
        /// <param name="empty">Provide the <see cref="OpposingPair"/> with neither value defined</param>
        /// <param name="resultMessages"></param>
        /// <returns></returns>
        static private bool SolveSSA(ref OpposingPair complete, ref OpposingPair incomplete, ref OpposingPair empty, ref List<string> resultMessages)
        {
            bool a, b, c;

            // complete the first pair using the complete pair
            a = incomplete.GetAngle(complete, ref resultMessages);

            // find the last angle
            b = empty.GetAngle(complete.Vertex, incomplete.Vertex, ref resultMessages);

            // find the last side
            c = empty.GetSide(complete, ref resultMessages);

            return (a && b && c);
        }

        /// <summary>
        /// Attempts to find the last angle provided two angles are defined
        /// </summary>
        /// <param name="resultMessages"></param>
        /// <returns></returns>
        private bool CompleteAngles(ref List<string> resultMessages)
        {
            if (A.Vertex.Degrees + B.Vertex.Degrees + C.Vertex.Degrees > 180)
            {
                resultMessages.Add("All angles cannot add up to greater than 180 degrees.");
                return false;
            }
            if (A.Vertex.Degrees + B.Vertex.Degrees >= 180 ||
                A.Vertex.Degrees + C.Vertex.Degrees >= 180 ||
                B.Vertex.Degrees + C.Vertex.Degrees >= 180)
            {
                resultMessages.Add("Two angles cannot add up to greater than or equal to 180 degrees.");
                return false;
            }

            // try to get C angle
            if (C.GetAngle(B.Vertex, A.Vertex, ref resultMessages))
            {
                return true;
            }
            // try to get B angle
            else if (B.GetAngle(A.Vertex, C.Vertex, ref resultMessages))
            {
                return true;
            }
            // try to get A angle
            else if (A.GetAngle(B.Vertex, C.Vertex, ref resultMessages))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculate the alternate triangle using the main triangle
        /// </summary>
        /// <param name="resultMessages"></param>
        private void CalculateAltTriangle(ref List<string> resultMessages)
        {
            if (A.Vertex.Degrees == 90 || B.Vertex.Degrees == 90)
            {
                // No alternate triangle for this purpose
                AltTriangle = null;
                return;
            }

            // define new opposing pairs to define alt triangle from main triangle
            OpposingPair altD = new();
            OpposingPair altE = new();
            OpposingPair altF = new();

            if (A.Vertex.Degrees <= 90 && B.Vertex.Degrees <= 90)
            {
                // internal alternative triangle
                altD.Vertex.Degrees = 90;
                altE.Vertex.Degrees = B.Vertex.Degrees;
                altD.Side.Value = A.Side.Value;
            }
            else
            {
                // external alternative triangle
                altD.Vertex.Degrees = 90;
                if (A.Vertex.Degrees > B.Vertex.Degrees)
                {
                    // on the right side
                    altD.Side.Value = B.Side.Value;
                    altE.Vertex.Degrees = 180 - A.Vertex.Degrees;
                }
                else
                {
                    // on the left side
                    altD.Side.Value = A.Side.Value;
                    altE.Vertex.Degrees = 180 - B.Vertex.Degrees;
                }
            }

            // define alternate triangle using complementary opposing pairs
            AltTriangle = new TriangleSolver(altD, altE, altF);

            // solve using Angle Side Angle method
            AltTriangle.SolveASA(ref resultMessages);

        }


        /// <summary>
        /// Returns a string that displays side and vertex details of A, B, and C.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return ("A: " + A.ToString() + " B: " + B.ToString() + " C: " + C.ToString());
        }

    } // Triangle Solver


    /// <summary>
    /// A <see cref="TriangleWorkarea"/> contains the rectangle border,
    /// <br>Plus the algorithm to draw the triangle within it.</br>
    /// <para>It should resize the triangle to fit the workarea after the form resizes.</para>
    /// </summary>
    internal class TriangleWorkarea : IDisposable
    {
        private bool _disposed = false;

        public void Dispose()
        {
            if (!_disposed)
            {
                // Unsubscribe from the Resize event
                _form.Resize -= OnResize;

                _disposed = true;
            }
        }

        private readonly FormMain _form;

        /// <summary>
        /// The original triangle calcualted using trig based on user input
        /// </summary>
        private TriangleSolver? _mainTriangle;

        /// <summary>
        /// Copy of the original triangle scaled to fit the workarea based on form size
        /// </summary>
        private TriangleSolver? _scaleTriangle;

        /// <summary>
        /// Fields defined only from the ImportTriangle method used to determine ratio
        /// </summary>
        private double _actualHeight, _actualWidth;

        // private pointTrio fields for reference properties
        private PointTrio _mainPoints;
        private PointTrio _altPoints;

        /// <summary>
        /// Trio of points that defines the main triangle
        /// </summary>
        internal ref PointTrio MainPoints
        {
            // ref keyword allows this property to be used as a reference parameter
            get { return ref _mainPoints; }
        }

        /// <summary>
        /// Trio of points that defines the alternate triangle
        /// </summary>
        internal ref PointTrio AltPoints
        {
            // ref keyword allows this property to be used as a reference parameter
            get { return ref _altPoints; }
        }

        /// <summary>
        /// Get left edge
        /// </summary>
        static internal int Left
        {
            get { return FRM.BORDER2 + TXT.WIDTH; }
        }

        /// <summary>
        /// Get right edge
        /// </summary>
        internal int Right
        {
            get { return _form.ClientRectangle.Width - Left; }
        }

        /// <summary>
        /// Get workarea width
        /// </summary>
        internal int Width
        {
            get { return Right - Left; }
        }

        /// <summary>
        /// Get top edge
        /// </summary>
        static internal int Top
        {
            get { return FRM.BORDER2 + TXT.HEIGHT; }
        }

        /// <summary>
        /// Get bottom edge
        /// </summary>
        internal int Bottom
        {
            get { return _form.ClientRectangle.Height - Top; }
        }

        /// <summary>
        /// Get workarea height
        /// </summary>
        internal int Height
        {
            get { return Bottom - Top; }
        }

        /// <summary>
        /// Gets the workarea bounds to suit current form size  
        /// </summary>
        internal Rectangle Bounds
        {
            get { return new Rectangle(Left, Top, Width, Height); }
        }

        ///// <summary>
        ///// Returns the points of the triangle
        ///// </summary>
        ///// <returns></returns>
        //internal Point[] PointArray
        //{
        //    get { return [_p1, _p2, _p3]; }
        //}

        /// <summary>
        ///  do i allow a workarea without a triangle?
        /// </summary>
        /// <param name="form"></param>
        internal TriangleWorkarea(FormMain form)
        {
            _form = form;

            _mainTriangle = null;
            _scaleTriangle = null;

            _mainPoints = new();
            _altPoints = new();

            _form.Resize += OnResize; // add resize handler
        }

        /// <summary>
        /// Create copy of existing triangle and scale to suit workarea
        /// </summary>
        /// <param name="triangle"></param>
        internal void ImportTriangle(TriangleSolver triangle)
        {
            // store the original triangle at correct size
            _mainTriangle = new TriangleSolver(triangle);

            // copy triangle so it can be scaled to fit later
            _scaleTriangle = new TriangleSolver(triangle);

            // determine _actualWidth and _actualHeight based on conditions
            if (_mainTriangle.A.Vertex.Degrees == 90)
            {
                // Right triangle with A as the right angle (on the right)
                _actualHeight = _mainTriangle.B.Side.Value;
                _actualWidth = _mainTriangle.C.Side.Value;
            }
            else if (_mainTriangle.B.Vertex.Degrees == 90)
            {
                // Right triangle with B as the right angle (on the left)
                _actualHeight = _mainTriangle.A.Side.Value;
                _actualWidth = _mainTriangle.C.Side.Value;
            }
            else if (_mainTriangle.A.Vertex.Degrees <= 90 && _mainTriangle.B.Vertex.Degrees <= 90)
            {
                // Internal alternate triangle when both angles are less than 90 - only determines height
                _actualHeight = _mainTriangle.AltTriangle?.B.Side.Value ?? 0;
                _actualWidth = _mainTriangle.C.Side.Value;
            }
            else
            {
                // External alternate triangle when one angle is greater than 90 - determines height and width
                _actualHeight = _mainTriangle.AltTriangle?.B.Side.Value ?? 0;
                _actualWidth = _mainTriangle.C.Side.Value + (_mainTriangle.AltTriangle?.C.Side.Value ?? 0);
            }

            // Scale the triangle
            Scale();

            // Calculate the points for the main triangle
            GetPoints(_scaleTriangle, ref MainPoints);

            if (_scaleTriangle.AltTriangle != null)
            {
                // calculate alternate points too
                GetPoints(_scaleTriangle.AltTriangle, ref AltPoints);
            }
        }

        /// <summary>
        /// Scale the triangle to fit within the workarea
        /// </summary>
        void Scale()
        {
            if (_scaleTriangle != null)
            {
                // Determine the available space in the workarea
                double availableWidth = Width - FRM.INSET2;
                double availableHeight = Height - FRM.INSET2;

                // Calculate the scaling factor
                double widthScale = availableWidth / _actualWidth;
                double heightScale = availableHeight / _actualHeight;
                double scaleFactor = Math.Min(widthScale, heightScale);

                // update the Scale triangle to suit the scale factor
                _scaleTriangle?.Scale(scaleFactor);

                Debug.WriteLine($"Scale Factor: {scaleFactor}, Actual Width: {_actualWidth}, Height: {_actualHeight}");
            }
        }


        /// <summary>
        /// Calculate the points of the triangle
        /// </summary>
        /// <param name="triangle">Specify the triangle data from which points are derived</param>
        /// <param name="trio">Specify the reference in which points will be returned</param>
        void GetPoints(TriangleSolver triangle, ref PointTrio trio)
        {
            if (triangle == null)
            {
                // clear all points
                trio.Clear();

                // exit void
                return;
            }

            // Start P1 (Bottom Left) at the default position
            int p1_x = Left + FRM.INSET;
            int p1_y = Bottom - FRM.INSET;

            // Adjust P1 if Angle B > 90
            if (triangle.B.Vertex.Degrees > 90 && triangle.AltTriangle != null)
            {
                p1_x += (int)triangle.AltTriangle.C.Side.Value;
            }

            trio.P1 = new Point(p1_x, p1_y);

            // Place P2 (Bottom Right) along the x-axis
            int p2_x = trio.P1.X + (int)triangle.C.Side.Value;
            int p2_y = trio.P1.Y;
            trio.P2 = new Point(p2_x, p2_y);

            // Calculate P3 (Top) using _p2, angle A, and side B
            double angleA = triangle.A.Vertex.Radians;
            double sideB = triangle.B.Side.Value;
            int p3_x_from_p2 = trio.P2.X - (int)(sideB * Math.Cos(angleA));
            int p3_y_from_p2 = trio.P2.Y - (int)(sideB * Math.Sin(angleA));
            Point p3_from_p2 = new Point(p3_x_from_p2, p3_y_from_p2);

            // Calculate P3 (Top) using _p1, angle B, and side A
            double angleB = triangle.B.Vertex.Radians;
            double sideA = triangle.A.Side.Value;
            int p3_x_from_p1 = trio.P1.X + (int)(sideA * Math.Cos(angleB));
            int p3_y_from_p1 = trio.P1.Y - (int)(sideA * Math.Sin(angleB));
            Point p3_from_p1 = new Point(p3_x_from_p1, p3_y_from_p1);

            // Use one of the calculated points as the final P3
            trio.P3 = p3_from_p2; // should be the same or very close

            Debug.WriteLine($"_p1: {trio.P1}, _p2: {trio.P2}, _p3a: {p3_from_p1}, _p3b {p3_from_p2}");
        }

        void OnResize(object? sender, EventArgs e)
        {
            // recalculate the workarea bounds
            Scale();

            if (_scaleTriangle != null)
            {
                // Calculate the points for the main triangle
                GetPoints(_scaleTriangle, ref MainPoints);

                if (_scaleTriangle.AltTriangle != null)
                {
                    // calculate alternate points too
                    GetPoints(_scaleTriangle.AltTriangle, ref AltPoints);
                }
            }
        }


    } // Workarea



    /// <summary>
    /// Collection of points that can be used to define a triangle
    /// </summary>
    internal class PointTrio
    {
        internal Point P1 { get; set; }
        internal Point P2 { get; set; }
        internal Point P3 { get; set; }

        /// <summary>
        /// Define empty collection of points
        /// </summary>
        internal PointTrio()
        {
            P1 = new();
            P2 = new();
            P3 = new();
        }

        /// <summary>
        /// Define collection of points that can be used to define a triangle
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        internal PointTrio(Point p1, Point p2, Point p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        /// <summary>
        /// Define collection of points that can be used to define a triangle
        /// </summary>
        /// <param name="pointTrio"></param>
        internal PointTrio(PointTrio pointTrio)
        {
            P1 = pointTrio.P1;
            P2 = pointTrio.P2;
            P3 = pointTrio.P3;
        }

        /// <summary>
        /// Gets value indicating whether any points are empty
        /// </summary>
        internal bool IsEmpty
        {
            get
            {
                return P1.IsEmpty || P2.IsEmpty || P3.IsEmpty;
            }
        }

        internal void Clear()
        {
            P1 = new();
            P2 = new();
            P3 = new();
        }

        /// <summary>
        /// Returns the trio of <see cref="Point"/> objects as an array
        /// </summary>
        /// <returns></returns>
        internal Point[] ToArray()
        {
            return [P1, P2, P3];
        }

    } // PointTrio

}


