using System.Diagnostics;

namespace triggy
{
    /// <summary>
    /// Describe completion status of <see cref="OpposingPair"/> objects.
    /// </summary>
    enum PairStatus
    {
        NEITHER = 0,
        SIDE = 1,
        ANGLE = 2,
        BOTH = 3
    };

    public struct ERRMSG
    {
        public const string OneAngle180 = "One angle cannot be greater than 180 degrees.";
        public const string TwoAngle180 = "Two angles cannot total greater than 180 degrees.";
        public const string OneSide = "One side cannot be longer than the other two sides.";
        public const string AngleTooLarge = "Angle is too large. No intersection could be found.";
    }

    public struct FRM
    {
        public const int HEIGHT = 600;          // height
        public const int WIDTH = 800;           // width
        public const int BORDER = 6;            // border
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
        public const int HEIGHT = 34; // button height
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
    /// Angle that calculates both degrees and radians.
    /// <para>Operators use degrees value by default.</para>
    /// </summary>
    internal class Angle : IEquatable<Angle>, IComparable<Angle>
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
        internal bool IsEmpty
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
        /// Returns the current value to zero
        /// </summary>
        internal void Clear()
        {
            Degrees = 0;
        }

        public override int GetHashCode()
        {
            return Degrees.GetHashCode();
        }

        // compare against angle
        public bool Equals(Angle? other)
        {
            if (other is null) return false;
            return Degrees == other.Degrees;
        }

        // compare against double

        /// <summary>
        /// Indicates whether the current <see cref="Angle"/> is equal to the equivalent <see cref="double"/> value
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(double other)
        {
            return Degrees == other;
        }

        // compare packaged objects                
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (obj is not Angle other) return false;
            return Equals(other);
        }

        // compare tos

        public int CompareTo(Angle? other)
        {
            if (other is null) return 1;
            return Degrees.CompareTo(other.Degrees);
        }

        public int CompareTo(double other)
        {
            return Degrees.CompareTo(other);
        }

        // angle operators

        public static bool operator ==(Angle left, Angle right)
        {
            // use default equality check between Angle objects
            return EqualityComparer<Angle>.Default.Equals(left, right);
        }
        public static bool operator ==(Angle left, double right)
        {
            return left.Equals(right);
        }
        public static bool operator ==(double left, Angle right)
        {
            return left.Equals(right.Degrees);
        }

        public static bool operator !=(Angle left, Angle right)
        {
            return !(left == right);
        }
        public static bool operator !=(Angle left, double right)
        {
            return !left.Equals(right);
        }
        public static bool operator !=(double left, Angle right)
        {
            return !left.Equals(right.Degrees);
        }

        public static bool operator <(Angle left, Angle right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <(Angle left, double right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <(double left, Angle right)
        {
            return left.CompareTo(right.Degrees) < 0;
        }

        public static bool operator >(Angle left, Angle right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >(Angle left, double right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >(double left, Angle right)
        {
            return left.CompareTo(right.Degrees) > 0;
        }

        public static bool operator <=(Angle left, Angle right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator <=(Angle left, double right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator <=(double left, Angle right)
        {
            return left.CompareTo(right.Degrees) <= 0;
        }

        public static bool operator >=(Angle left, Angle right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator >=(Angle left, double right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator >=(double left, Angle right)
        {
            return left.CompareTo(right.Degrees) >= 0;
        }

        public static double operator +(Angle left, Angle right)
        {
            return left.Degrees + right.Degrees;
        }
        public static double operator +(Angle left, double right)
        {
            return left.Degrees + right;
        }
        public static double operator +(double left, Angle right)
        {
            return left + right.Degrees;
        }

        public static double operator -(Angle left, Angle right)
        {
            return left.Degrees - right.Degrees;
        }
        public static double operator -(Angle left, double right)
        {
            return left.Degrees - right;
        }
        public static double operator -(double left, Angle right)
        {
            return left - right.Degrees;
        }
       
        public static double operator *(Angle left, Angle right)
        {
            return left.Degrees * right.Degrees;
        }
        public static double operator *(Angle left, double right)
        {
            return left.Degrees * right;
        }
        public static double operator *(double left, Angle right)
        {
            return left * right.Degrees;
        }

        public static double operator /(Angle left, Angle right)
        {
            if (right.Degrees == 0)
            {
                throw new DivideByZeroException("Cannot divide by an Angle with a value of zero degrees.");
            }
            return left.Degrees / right.Degrees;
        }
        public static double operator /(Angle left, double right)
        {
            if (right == 0)
            {
                throw new DivideByZeroException("Cannot divide by zero.");
            }
            return left.Degrees / right;
        }
        public static double operator /(double left, Angle right)
        {
            if (right.Degrees == 0)
            {
                throw new DivideByZeroException("Cannot divide by an Angle with a value of zero degrees.");
            }
            return left / right.Degrees;
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
    internal class Length : IEquatable<Length>, IComparable<Length>
    {
        /// <summary>
        /// Gets or sets the side Value
        /// </summary>
        internal double Value { get; set; }

        /// <summary>
        /// Gets the value squared or sets the squareroot value
        /// <para>Updated to accept a Set method as well</para>
        /// </summary>
        internal double Squared
        {
            get
            {
                // return value to power of 2
                return Math.Pow(Value, 2);
            }
            set
            {
                // store the square root of value
                Value = Math.Sqrt(value);
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
        internal bool IsEmpty
        {
            get
            {
                return Value == 0;
            }
        }

        /// <summary>
        /// Returns the current value to zero
        /// </summary>
        internal void Clear()
        {
            Value = 0;
        }

        // equality checks

        public bool Equals(Length? other)
        {
            if (other is null) return false;
            return Value == other.Value;
        }

        /// <summary>
        /// Indicates whether the current <see cref="Length"/> is equal to the equivalent <see cref="double"/> value
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(double other)
        {
            return Value == other;
        }

        // compare packaged objects                
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (obj is not Length other) return false;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        // compare tos

        public int CompareTo(Length? other)
        {
            if (other is null) return 1;
            return Value.CompareTo(other.Value);
        }

        public int CompareTo(double other)
        {
            return Value.CompareTo(other);
        }

        // lengths operators

        public static bool operator ==(Length left, Length right)
        {
            return EqualityComparer<Length>.Default.Equals(left, right);
        }
        public static bool operator ==(Length left, double right)
        {
            return left.Equals(right);
        }
        public static bool operator ==(double left, Length right)
        {
            return left.Equals(right.Value);
        }

        public static bool operator !=(Length left, Length right)
        {
            return !(left == right);
        }
        public static bool operator !=(Length left, double right)
        {
            return !left.Equals(right);
        }
        public static bool operator !=(double left, Length right)
        {
            return !left.Equals(right.Value);
        }

        public static bool operator <(Length left, Length right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <(Length left, double right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <(double left, Length right)
        {
            return left.CompareTo(right.Value) < 0;
        }

        public static bool operator >(Length left, Length right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >(Length left, double right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >(double left, Length right)
        {
            return left.CompareTo(right.Value) > 0;
        }

        public static bool operator <=(Length left, Length right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator <=(Length left, double right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator <=(double left, Length right)
        {
            return left.CompareTo(right.Value) <= 0;
        }

        public static bool operator >=(Length left, Length right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator >=(Length left, double right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator >=(double left, Length right)
        {
            return left.CompareTo(right.Value) >= 0;
        }

        public static double operator +(Length left, Length right)
        {
            return left.Value + right.Value;
        }
        public static double operator +(Length left, double right)
        {
            return left.Value + right;
        }
        public static double operator +(double left, Length right)
        {
            return left + right.Value;
        }

        public static double operator -(Length left, Length right)
        {
            return left.Value - right.Value;
        }
        public static double operator -(Length left, double right)
        {
            return left.Value - right;
        }
        public static double operator -(double left, Length right)
        {
            return left - right.Value;
        }

        public static double operator *(Length left, Length right)
        {
            return left.Value * right.Value;
        }
        public static double operator *(Length left, double right)
        {
            return left.Value * right;
        }
        public static double operator *(double left, Length right)
        {
            return left * right.Value;
        }

        public static double operator /(Length left, Length right)
        {
            if (right.Value == 0)
            {
                throw new DivideByZeroException("Cannot divide by a Length with a value of zero.");
            }
            return left.Value / right.Value;
        }
        public static double operator /(Length left, double right)
        {
            if (right == 0)
            {
                throw new DivideByZeroException("Cannot divide by zero.");
            }
            return left.Value / right;
        }
        public static double operator /(double left, Length right)
        {
            if (right.Value == 0)
            {
                throw new DivideByZeroException("Cannot divide by a Length with a value of zero.");
            }
            return left / right.Value;
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

        /// <summary>
        /// Returns all points to their default empty value
        /// </summary>
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
                if (_txtVertex != null) _txtVertex.Text = string.Empty;
                if (_txtSide != null) _txtSide.Text = string.Empty;

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
        private readonly TextBox? _txtVertex = null;
        private readonly TextBox? _txtSide = null;

        /// <summary>
        /// Determine if this instance contains Side, Angle, Both or Neither
        /// </summary>
        internal PairStatus GetStatus
        {
            get
            {
                int result = (int)PairStatus.NEITHER;
                if (!Side.IsEmpty) { result += (int)PairStatus.SIDE; }
                if (!Vertex.IsEmpty) { result += (int)PairStatus.ANGLE; }
                return (PairStatus)result;
            }
        }

        /// <summary>
        /// Returns Vertex <see cref="Angle"/> and Side <see cref="Length"/> values to zero.
        /// </summary>
        internal void Clear()
        {
            Vertex.Clear();
            Side.Clear();
        }

        /// <summary>
        /// Determines if the side is defined
        /// </summary>
        /// <returns></returns>
        internal bool IsSideDefined()
        {
            return !Side.IsEmpty;
        }

        /// <summary>
        /// Determines if the side is defined
        /// </summary>
        /// <param name="accum">Accumulate number of sides</param>
        /// <returns></returns>
        internal bool IsSideDefined(ref int accum)
        {
            if (!Side.IsEmpty) { accum++; }
            return !Side.IsEmpty;
        }

        /// <summary>
        /// Determines if the angle is defined
        /// </summary>
        /// <returns></returns>
        internal bool IsVertexDefined()
        {
            return !Vertex.IsEmpty;
        }

        /// <summary>
        /// Determines if the angle is defined
        /// </summary>
        /// <param name="accum">Accumulate number of angles</param>
        /// <returns></returns>
        internal bool IsVertexDefined(ref int accum)
        {
            if (!Vertex.IsEmpty) { accum++; }
            return !Vertex.IsEmpty;
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

            // empty textboxes
            _txtSide = null;
            _txtVertex = null;
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
            _txtVertex = vertex;
            _txtSide = side;
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

            // empty textboxes
            _txtSide = null;
            _txtVertex = null;
        }

        /// <summary>
        /// Get the remaining vertex angle using other vertex angles
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        internal bool GetAngle(Angle vertexA, Angle vertexB, ref HashSet<string> resultMessages)
        {
            if (!vertexA.IsEmpty && !vertexB.IsEmpty)
            {
                // check combined angles
                if (vertexA + vertexB < 180)
                {
                    // calculate the angle
                    Vertex.Degrees = 180 - vertexA - vertexB;

                    // return success
                    return true;
                }
                else
                {
                    // record error
                    resultMessages.Add(ERRMSG.TwoAngle180);
                }
            }

            // return incomplete
            return false;
        }

        /// <summary>
        /// Get current vertex angle using complete <see cref="OpposingPair"/>
        /// <br>Requires side value to be defined in the current <see cref="OpposingPair"/></br>
        /// </summary>
        internal bool GetAngle(OpposingPair pair, ref HashSet<string> resultMessages)
        {
            if (!Side.IsEmpty && pair.GetStatus == PairStatus.BOTH)
            {
                double _asin = (Side * Math.Sin(pair.Vertex.Radians)) / pair.Side;

                if (_asin >= -1 && _asin <= 1)
                {
                    // calculate result in radians
                    Vertex.Radians = Math.Asin(_asin);

                    // angle must be between 0 and 180 (not inclusive)
                    if (Vertex > 0 && Vertex < 180)
                    {
                        // ensure angle found is within accepted limits
                        return true;
                    }
                    else
                    {
                        // determine what causes this error
                        resultMessages.Add(ERRMSG.AngleTooLarge);
                    }
                }
                else
                {
                    // determine what causes this error
                    resultMessages.Add(ERRMSG.AngleTooLarge);
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
        internal bool GetAngle(Length sideB, Length sideC)
        {
            if (!Side.IsEmpty)
            {
                // calculate result in radians
                Vertex.Radians = Math.Acos((sideB.Squared + sideC.Squared - Side.Squared) / (2 * sideB * sideC));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks side lengths against those in other <see cref="OpposingPair"/>(s)
        /// </summary>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="returnMessage"></param>
        /// <returns></returns>
        internal bool CheckSide(OpposingPair pairB, OpposingPair pairC, ref HashSet<string> returnMessage)
        {
            // check side length against sum of other two sides
            if (Side > pairB.Side + pairC.Side)
            {
                returnMessage.Add(ERRMSG.OneSide);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get side value using complete <see cref="OpposingPair"/>
        /// <br>Requires vertex angle to be defined in the current <see cref="OpposingPair"/></br>
        /// </summary>
        /// <param name="C"></param>
        internal bool GetSide(OpposingPair pairB, ref HashSet<string> resultMessages)
        {
            if (!Vertex.IsEmpty && pairB.GetStatus == PairStatus.BOTH)
            {
                // ensure angle is valid
                if (Vertex < 180)
                {
                    // calculate own side using another complete opposing pair and own angle
                    Side.Value = (pairB.Side * Math.Sin(Vertex.Radians)) / Math.Sin(pairB.Vertex.Radians);

                    return true;
                }
                else
                {
                    // record error
                    resultMessages.Add(ERRMSG.OneAngle180);
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
        internal bool GetSide(Length sideB, Length sideC, ref HashSet<string> resultMessages)
        {
            // ensure angle and both lengths are defined
            if (!Vertex.IsEmpty && !sideB.IsEmpty && !sideC.IsEmpty)
            {
                // ensure angle is within limits
                if (Vertex < 180)
                {
                    // calculate own side using adjacent sides and own angle
                    Side.Squared = sideB.Squared + sideC.Squared - 2 * sideB * sideC * Math.Cos(Vertex.Radians);

                    return true;
                }
                else
                {
                    // record error
                    resultMessages.Add(ERRMSG.OneAngle180);
                }
            }

            return false;
        }

        /// <summary>
        /// Display the opposing pair values in the provided textboxes
        /// </summary>
        internal void SetTextBoxValues()
        {
            if (_txtVertex != null)
            {
                // clear if zero, otherwise set value
                _txtVertex.Text = Vertex == 0 ? string.Empty : Vertex.ToString();
            }

            if (_txtSide != null)
            {
                // clear if zero, otherwise set value
                _txtSide.Text = Side.Value == 0 ? string.Empty : Side.ToString();
            }

        }

        /// <summary>
        /// Read the provided textbox values into the opposing pairs
        /// </summary>
        internal void GetTextBoxValues()
        {
            if (_txtVertex != null) Vertex.Degrees = _txtVertex.ToDouble();
            if (_txtSide != null) Side.Value = _txtSide.ToDouble();
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

        // private fields

        // Main triangle pairs
        private OpposingPair _a;
        private OpposingPair _b;
        private OpposingPair _c;

        // Fields used to store original sides for dynamic scaling
        private double _origSideA;
        private double _origSideB;
        private double _origSideC;

        // reference properties

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

        // constructors

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
            }
        }

        // internal properties

        /// <summary>
        /// Returns a string array of results for both the main and alternate triangles
        /// </summary>
        /// <returns></returns>
        internal string[] ResultList
        {
            get
            {
                List<string> result =
                [
                    $"A° {_a.Vertex}",
                    $"a  {_a.Side}",
                    $"B° {_b.Vertex}",
                    $"b  {_b.Side}",
                    $"C° {_c.Vertex}",
                    $"c  {_c.Side}",
                ];

                if (AltTriangle != null)
                {
                    result.Add("");
                    result.Add($"D° {AltTriangle._a.Vertex}");
                    result.Add($"d  {AltTriangle._a.Side}");
                    result.Add($"E° {AltTriangle._b.Vertex}");
                    result.Add($"e  {AltTriangle._b.Side}");
                    result.Add($"F° {AltTriangle._c.Vertex}");
                    result.Add($"f  {AltTriangle._c.Side}");
                }

                return result.ToArray();
            }
        }

        /// <summary>
        /// Determines if the current instance contains sufficient data to solve
        /// <p>Does not yet check validity of data</p>
        /// </summary>
        /// <returns></returns>
        internal bool IsSufficient
        {
            get
            {
                int accumLength = 0;
                int accumAngle = 0;

                // count pairs with side length defined
                A.IsSideDefined(ref accumLength);
                B.IsSideDefined(ref accumLength);
                C.IsSideDefined(ref accumLength);

                // count pairs with vertex angle defined
                A.IsVertexDefined(ref accumAngle);
                B.IsVertexDefined(ref accumAngle);
                C.IsVertexDefined(ref accumAngle);

                // any combination of three values is okay
                if (accumAngle + accumLength > 2) { return true; }

                // any two vertex angles is okay
                if (accumAngle > 1) { return true; }

                // insufficient
                return false;
            }
        }

        /// <summary>
        /// Determines if the current instance contains all completed data
        /// </summary>
        internal bool IsComplete
        {
            get
            {
                return _a.GetStatus == PairStatus.BOTH && _b.GetStatus == PairStatus.BOTH && _c.GetStatus == PairStatus.BOTH;
            }
        }

        // internal methods

        /// <summary>
        /// Determine if preliminary angle input data is valid
        /// </summary>
        /// <param name="resultMessages"></param>
        /// <returns></returns>
        internal bool CheckAngles(ref HashSet<string> resultMessages)
        {
            // ensure no one angle is greater than 180
            if (A.Vertex > 180 || B.Vertex > 180 && C.Vertex > 180)
            {
                resultMessages.Add(ERRMSG.OneAngle180);
                return false;
            }

            // ensure no two angles are greater than 180
            if (A.Vertex + B.Vertex > 180 ||
                B.Vertex + C.Vertex > 180 ||
                A.Vertex + C.Vertex > 180)
            {
                resultMessages.Add(ERRMSG.TwoAngle180);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Store original sides if side values are not empty
        /// </summary>
        internal void SetOriginalSides()
        {
            _origSideA = _a.Side.Value;
            _origSideB = _b.Side.Value;
            _origSideC = _c.Side.Value;
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

        /// <summary>
        /// Returns all vertex <see cref="Angle"/> and side <see cref="Length"/> values to zero
        /// </summary>
        internal void Clear()
        {
            _a.Clear();
            _b.Clear();
            _c.Clear();

            // remove
            AltTriangle = null;
        }

        /// <summary>
        /// Scale the side lengths of the triangle by a given factor
        /// <br>Also scales AltTriangle if available</br>
        /// </summary>
        /// <param name="factor"></param>
        internal void Scale(double factor)
        {
            // only scale if all sides are defined
            // if a solve call returns an error, some sides may not get calculated
            if (_origSideA > 0 && _origSideB > 0 && _origSideC > 0)
            {
                // Scale main triangle
                _a.Side.Value = _origSideA * factor;
                _b.Side.Value = _origSideB * factor;
                _c.Side.Value = _origSideC * factor;

                // Scale alternate triangle if not null
                AltTriangle?.Scale(factor);
            }
        }

        // solve methods

        /// <summary>
        /// Solve triangle using any two angles and one side
        /// </summary>
        /// <param name="resultMessages"></param>
        /// <returns></returns>
        internal bool SolveASA(ref HashSet<string> resultMessages)
        {
            // if one side and any two angles are given

            // first complete angles if possible
            if (CalculateAngles(ref resultMessages))
            {
                // quick check if all sides are empty and assign one
                if (_a.Side.IsEmpty && _b.Side.IsEmpty && _c.Side.IsEmpty)
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
        internal bool SolveSSS(ref HashSet<string> resultMessages)
        {
            if (!_a.Side.IsEmpty && !_b.Side.IsEmpty && !_c.Side.IsEmpty)
            {
                if (_a.CheckSide(_b, _c, ref resultMessages) &&
                    _b.CheckSide(_a, _c, ref resultMessages) &&
                    _c.CheckSide(_a, _b, ref resultMessages))
                {
                    if (_a.GetAngle(_b.Side, _c.Side) &&
                        _b.GetAngle(_a.Side, _c.Side) &&
                        _c.GetAngle(_a.Side, _b.Side))
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
        internal bool SolveSAS(ref HashSet<string> resultMessages)
        {
            if (!_a.Side.IsEmpty && !_b.Vertex.IsEmpty && !_c.Side.IsEmpty)
            {
                if (SolveSAS(ref _a, ref _b, ref _c, ref resultMessages))
                {
                    CalculateAltTriangle(ref resultMessages);
                    return true;
                }
            }
            else if (!_a.Side.IsEmpty && !_c.Vertex.IsEmpty && !_b.Side.IsEmpty)
            {
                if (SolveSAS(ref _a, ref _c, ref _b, ref resultMessages))
                {
                    CalculateAltTriangle(ref resultMessages);
                    return true;
                }
            }
            else if (!_b.Side.IsEmpty && !_a.Vertex.IsEmpty && !_c.Side.IsEmpty)
            {
                if (SolveSAS(ref _b, ref _a, ref _c, ref resultMessages))
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
        static private bool SolveSAS(ref OpposingPair sidePair1, ref OpposingPair anglePair, ref OpposingPair sidePair2, ref HashSet<string> resultMessages)
        {
            bool a, b, c;

            // first get the side opposite the angle
            b = anglePair.GetSide(sidePair1.Side, sidePair2.Side, ref resultMessages);

            // depends which side is smaller
            if (sidePair1.Side <= sidePair2.Side)
            {
                // complete angle pair 1
                a = sidePair1.GetAngle(anglePair, ref resultMessages);

                // complete remaining angle
                c = sidePair2.GetAngle(anglePair.Vertex, sidePair1.Vertex, ref resultMessages);
            }
            else
            {
                // complete angle pair 2
                a = sidePair2.GetAngle(anglePair, ref resultMessages);

                // complete remaining angle
                c = sidePair1.GetAngle(anglePair.Vertex, sidePair2.Vertex, ref resultMessages);
            }

            return (a && b && c);
        }

        /// <summary>
        /// Solve triangle using one angle and two sides, when angle <u>is not</u> between two adjacent sides
        /// </summary>
        /// <param name="resultMessages"></param>
        /// <returns></returns>
        internal bool SolveSSA(ref HashSet<string> resultMessages)
        {
            // if two sides and one angle are given, when the angle is not between adjacent sides

            OpposingPair? _complete = null;
            OpposingPair? _incomplete = null;
            OpposingPair? _empty = null;

            // determine which opposing pair is complete
            if (_a.GetStatus == PairStatus.BOTH)
            {
                _complete = _a;

                // if B side is defined, then use A to complete B
                if (!_b.Side.IsEmpty)
                {
                    _incomplete = _b;
                    _empty = _c;
                }
                // if C side is defined, then use C to complete B
                else if (!_c.Side.IsEmpty)
                {
                    _incomplete = _c;
                    _empty = _b;
                }
            }
            else if (_b.GetStatus == PairStatus.BOTH)
            {
                _complete = _b;

                // if A side is defined, then use B to complete A
                if (!_a.Side.IsEmpty)
                {
                    _incomplete = _a;
                    _empty = _c;
                }
                // if C side is defined, then use B to complete C
                else if (!_c.Side.IsEmpty)
                {
                    _incomplete = _c;
                    _empty = _a;
                }
            }
            else if (_c.GetStatus == PairStatus.BOTH)
            {
                _complete = _c;

                // if A side is defined, then use C to complete A
                if (!_a.Side.IsEmpty)
                {
                    _incomplete = _a;
                    _empty = _b;
                }
                // if B side is defined, then use C to complete B
                if (!_b.Side.IsEmpty)
                {
                    _incomplete = _b;
                    _empty = _a;
                }
            }

            // ensure we have each field assigned
            if (_complete != null && _incomplete != null && _empty != null)
            {
                // call static function with OpposingPairs in correct order
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
        static private bool SolveSSA(ref OpposingPair complete, ref OpposingPair incomplete, ref OpposingPair empty, ref HashSet<string> resultMessages)
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

        // private methods

        /// <summary>
        /// Attempts to find the last angle provided two angles are defined
        /// </summary>
        /// <param name="resultMessages"></param>
        /// <returns></returns>
        private bool CalculateAngles(ref HashSet<string> resultMessages)
        {
            if (_a.Vertex + _b.Vertex >= 180 ||
                _a.Vertex + _c.Vertex >= 180 ||
                _b.Vertex + _c.Vertex >= 180)
            {
                resultMessages.Add(ERRMSG.TwoAngle180);
                return false;
            }

            // try to get C angle
            if (_c.GetAngle(_a.Vertex, _b.Vertex, ref resultMessages))
            {
                return true;
            }
            // try to get B angle
            else if (_b.GetAngle(_a.Vertex, _c.Vertex, ref resultMessages))
            {
                return true;
            }
            // try to get A angle
            else if (_a.GetAngle(_b.Vertex, _c.Vertex, ref resultMessages))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculate the alternate triangle using the main triangle
        /// </summary>
        /// <param name="resultMessages"></param>
        private void CalculateAltTriangle(ref HashSet<string> resultMessages)
        {
            if (_a.Vertex == 90 || _b.Vertex == 90)
            {
                // No alternate triangle for this purpose
                AltTriangle = null;
                return;
            }

            // define new opposing pairs to define alt triangle from primary triangle
            OpposingPair altD = new();
            OpposingPair altE = new();
            OpposingPair altF = new();

            if (_a.Vertex <= 90 && _b.Vertex <= 90)
            {
                // internal alternative triangle
                altD.Vertex.Degrees = 90;
                altE.Vertex.Degrees = _b.Vertex.Degrees;
                altD.Side.Value = _a.Side.Value;
            }
            else
            {
                // external alternative triangle
                altD.Vertex.Degrees = 90;
                if (_a.Vertex > _b.Vertex)
                {
                    // on the right side
                    altD.Side.Value = _b.Side.Value;
                    altE.Vertex.Degrees = 180 - _a.Vertex;
                }
                else
                {
                    // on the left side
                    altD.Side.Value = _a.Side.Value;
                    altE.Vertex.Degrees = 180 - _b.Vertex;
                }
            }

            // define alternate triangle using complementary opposing pairs
            AltTriangle = new TriangleSolver(altD, altE, altF);

            // solve using Angle Side Angle method
            AltTriangle.SolveASA(ref resultMessages);

        }

        // public overrides

        /// <summary>
        /// Returns a string that displays side and vertex details of A, B, and C.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return ($"A: {_a}  B: {_b}  C: {_c}");
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
        /// The base triangle calcualted from user input using trigonometry
        /// </summary>
        private readonly TriangleSolver _baseTriangle;

        /// <summary>
        /// Copy of the base triangle to be scaled to fit the workarea
        /// </summary>
        private TriangleSolver _scaleTriangle;

        /// <summary>
        /// Fields defined only from the ImportTriangle method used to determine ratio
        /// </summary>
        private double _actualHeight, _actualWidth;

        // private pointTrio fields for reference properties
        private PointTrio _primaryPoints;
        private PointTrio _alternatePoints;

        /// <summary>
        /// Trio of points that defines the primary triangle
        /// </summary>
        internal ref PointTrio PrimaryPoints
        {
            // ref keyword allows this property to be used as a reference parameter
            get { return ref _primaryPoints; }
        }

        /// <summary>
        /// Trio of points that defines the alternate triangle
        /// </summary>
        internal ref PointTrio AlternatePoints
        {
            // ref keyword allows this property to be used as a reference parameter
            get { return ref _alternatePoints; }
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

        /// <summary>
        /// Initialise Workarea and prepare instances of base and scale triangles
        /// </summary>
        /// <param name="form"></param>
        /// <param name="triangle"></param>
        internal TriangleWorkarea(FormMain form, TriangleSolver triangle)
        {
            _form = form;

            // store original triangle instance
            _baseTriangle = triangle;
            _scaleTriangle = new(triangle);

            _primaryPoints = new();
            _alternatePoints = new();

            _form.Resize += OnResize; // add resize handler
        }

        /// <summary>
        /// Event handler function so resizing form automatically triggers the triangle to scale and point trio to be updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnResize(object? sender, EventArgs e)
        {
            // recalculate the workarea bounds
            Scale();

            // Calculate the points for the main triangle
            GetPoints(_scaleTriangle, ref PrimaryPoints, ref AlternatePoints);
        }

        /// <summary>
        /// Create copy of existing triangle and scale to suit workarea
        /// </summary>
        /// <param name="triangle"></param>
        internal void CarbonCopy()
        {
            // copy triangle so it can be scaled to fit later
            _scaleTriangle = new TriangleSolver(_baseTriangle);

            // check if triangle is sufficient before processing
            if (_baseTriangle.IsComplete)
            {
                // determine _actualWidth and _actualHeight based on conditions
                if (_baseTriangle.A.Vertex.Degrees == 90)
                {
                    // Right triangle with A as the right angle (on the right)
                    _actualHeight = _baseTriangle.B.Side.Value;
                    _actualWidth = _baseTriangle.C.Side.Value;
                }
                else if (_baseTriangle.B.Vertex.Degrees == 90)
                {
                    // Right triangle with B as the right angle (on the left)
                    _actualHeight = _baseTriangle.A.Side.Value;
                    _actualWidth = _baseTriangle.C.Side.Value;
                }
                else if (_baseTriangle.A.Vertex.Degrees <= 90 && _baseTriangle.B.Vertex.Degrees <= 90)
                {
                    // Internal alternate triangle when both angles are less than 90 - only determines height
                    _actualHeight = _baseTriangle.AltTriangle?.B.Side.Value ?? 0;
                    _actualWidth = _baseTriangle.C.Side.Value;
                }
                else
                {
                    // External alternate triangle when one angle is greater than 90 - determines height and width
                    _actualHeight = _baseTriangle.AltTriangle?.B.Side.Value ?? 0;
                    _actualWidth = _baseTriangle.C.Side.Value + (_baseTriangle.AltTriangle?.C.Side.Value ?? 0);
                }

                // Scale the triangle
                Scale();
            }

            // Calculate the points for the main triangle
            GetPoints(_scaleTriangle, ref PrimaryPoints, ref AlternatePoints);

        }

        /// <summary>
        /// Scale the triangle to fit within the workarea
        /// </summary>
        void Scale()
        {
            // Determine the available space in the workarea
            double availableWidth = Width - FRM.INSET2;
            double availableHeight = Height - FRM.INSET2;

            // Calculate the scaling factor
            double widthScale = availableWidth / _actualWidth;
            double heightScale = availableHeight / _actualHeight;
            double scaleFactor = Math.Min(widthScale, heightScale);

            // update the Scale triangle to suit the scale factor
            _scaleTriangle.Scale(scaleFactor);

            Debug.WriteLine($"Scale Factor: {scaleFactor}, Actual Width: {_actualWidth}, Height: {_actualHeight}");
        }

        /// <summary>
        /// Calculate all the points of the triangles
        /// </summary>
        /// <param name="triangle">Specify the triangle data from which points are derived</param>
        /// <param name="primaryPoints">Specify the reference to which primary points will be returned</param>
        /// <param name="alternatePoints">Specify the reference to which secondary points may be retured</param>
        void GetPoints(TriangleSolver triangle, ref PointTrio primaryPoints, ref PointTrio alternatePoints)
        {
            // clear all points to begin
            primaryPoints.Clear();
            alternatePoints.Clear();

            // determine if triangle contains complete data to calculate points
            if (triangle.IsComplete)
            {
                // Calculate Primary Points (standard calculation, fit triangle to workarea)
                primaryPoints = GetStandardPoints(triangle);

                // Calculate Alternate Points based on conditions
                if (triangle.AltTriangle != null && triangle.AltTriangle.IsSufficient)
                {
                    if (triangle.B.Vertex.Degrees > 90)
                    {
                        // Mirror the alternate triangle and place on left
                        alternatePoints = GetInvertedPoints(triangle.AltTriangle, primaryPoints.P1);
                    }
                    else if (triangle.A.Vertex.Degrees > 90)
                    {
                        // Offset the alternate triangle to place on right
                        alternatePoints = GetOffsetPoints(triangle.AltTriangle, primaryPoints.P1, (int)triangle.C.Side.Value);
                    }
                    else if (triangle.A.Vertex.Degrees < 90 && triangle.B.Vertex.Degrees < 90)
                    {
                        // Calculate alternate points using the same formula as the primary and place internally
                        alternatePoints = GetStandardPoints(triangle.AltTriangle);
                    }
                }
            }

        }

        /// <summary>
        /// Determine point trio within workarea to suit <paramref name="triangle"/> data
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns></returns>
        PointTrio GetStandardPoints(TriangleSolver triangle)
        {
            PointTrio points = new();

            // Start P1 (Bottom Left) at the default position
            int p1_x = Left + FRM.INSET;
            int p1_y = Bottom - FRM.INSET;

            // Adjust P1 if Angle B > 90
            if (triangle.B.Vertex.Degrees > 90 && triangle.AltTriangle != null)
            {
                p1_x += (int)triangle.AltTriangle.C.Side.Value;
            }

            points.P1 = new Point(p1_x, p1_y);

            // Place P2 (Bottom Right) along the x-axis
            int p2_x = points.P1.X + (int)triangle.C.Side.Value;
            int p2_y = points.P1.Y;
            points.P2 = new Point(p2_x, p2_y);

            // Calculate P3 (Top) using P2, angle A, and side B
            double angleA = triangle.A.Vertex.Radians;
            double sideB = triangle.B.Side.Value;
            int p3_x_from_p2 = points.P2.X - (int)(sideB * Math.Cos(angleA));
            int p3_y_from_p2 = points.P2.Y - (int)(sideB * Math.Sin(angleA));
            points.P3 = new Point(p3_x_from_p2, p3_y_from_p2);

            return points;
        }

        /// <summary>
        /// Invert <paramref name="triangle"/> point data (mirror) around <paramref name="referenceP1"/>
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="referenceP1"></param>
        /// <returns></returns>
        static PointTrio GetInvertedPoints(TriangleSolver triangle, Point referenceP1)
        {
            PointTrio points = new();

            // P1 remains the same as the reference point
            points.P1 = referenceP1;

            // P2 is mirrored to the left
            points.P2 = new Point(referenceP1.X - (int)triangle.C.Side.Value, referenceP1.Y);

            // P3 is calculated using the mirrored P2
            double angleA = triangle.A.Vertex.Radians;
            double sideB = triangle.B.Side.Value;
            int p3_x_from_p2 = points.P2.X + (int)(sideB * Math.Cos(angleA));
            int p3_y_from_p2 = points.P2.Y - (int)(sideB * Math.Sin(angleA));
            points.P3 = new Point(p3_x_from_p2, p3_y_from_p2);

            return points;
        }

        /// <summary>
        /// Offset <paramref name="triangle"/> point data from <paramref name="referenceP1"/> by an <paramref name="offset"/> amount
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="referenceP1"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        static PointTrio GetOffsetPoints(TriangleSolver triangle, Point referenceP1, int offset)
        {
            PointTrio points = new();

            // P1 is offset by the given distance
            points.P1 = new Point(referenceP1.X + offset, referenceP1.Y);

            // P2 is calculated based on the offset P1
            points.P2 = new Point(points.P1.X + (int)triangle.C.Side.Value, points.P1.Y);

            // P3 is calculated using the offset P2
            double angleA = triangle.A.Vertex.Radians;
            double sideB = triangle.B.Side.Value;
            int p3_x_from_p2 = points.P2.X - (int)(sideB * Math.Cos(angleA));
            int p3_y_from_p2 = points.P2.Y - (int)(sideB * Math.Sin(angleA));
            points.P3 = new Point(p3_x_from_p2, p3_y_from_p2);

            return points;
        }

    } // Workarea
}


