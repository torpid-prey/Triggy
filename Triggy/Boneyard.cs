using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using triggy;

namespace triggy
{
    class Boneyard
    {
        // this is just a file containing old removed code as I try to get ideas fleshed out.
        // its contents is entirely inconsequential to the application

        ///// <summary>
        ///// Define new default instance of TriangleSolver with an equalateral triangle
        ///// </summary>
        //internal TriangleSolver()
        //{
        //    //// define collection of reuslt messages
        //    //List<string> resultMessages = new();

        //    _a = new OpposingPair(); //{ Vertex = new Angle(60) };
        //    _b = new OpposingPair(); //{ Vertex = new Angle(60) };
        //    _c = new OpposingPair(); //{ Side = new Length(150) };

        //    //// consume resultMessages - there shouldn't be any with this triangle
        //    //SolveASA(ref resultMessages);

        //    //// store original side lengths for dynamic scaling
        //    //SetOriginalSides(_a, _b, _c);
        //}
        //int accum = 0;

        //a.HasSide(ref accum);
        //a.HasAngle(ref accum);
        //b.HasSide(ref accum);
        //b.HasAngle(ref accum);
        //c.HasSide(ref accum);
        //c.HasAngle(ref accum);

        //if ( accum > 3)
        //{
        //}
        //else
        //{
        //    // define collection of reuslt messages
        //    List<string> resultMessages = new();

        //    _a = new OpposingPair() { Vertex = new Angle(60) };
        //    _b = new OpposingPair() { Vertex = new Angle(60) };
        //    _c = new OpposingPair() { Side = new Length(150) };

        //    // consume resultMessages - there shouldn't be any with this triangle
        //    SolveASA(ref resultMessages);
        //}

        // store original side lengths for dynamic scaling
        //SetOriginalSides(_a, _b, _c);





        //struct EDGE
        //{
        //    public const int TOP = 44;
        //    public const int LEFT = 120;
        //    public const int BOTTOM = 65;
        //    public const int RIGHT = 130;
        //    public const int BORDER = 50;
        //}

        ///// <summary>
        ///// Gets the workarea bounds to suit current form size  
        ///// </summary>
        //internal Rectangle Bounds
        //{
        //    get
        //    {
        //        return new Rectangle
        //        (
        //            EDGE.LEFT,
        //            EDGE.TOP,
        //            _form.Width - 2 * EDGE.RIGHT,
        //            _form.Height - 2 * EDGE.BOTTOM
        //        );
        //    }
        //}


        // get number of complete angles and number of compelte sides


        // if 2 angles and a side, or 2 sides and an angle, or 3 sides are given, try to complete triangle

        ///// <summary>
        ///// Determines how many <see cref="Opposite"/> Pairs contain Side information
        ///// </summary>
        ///// <param name="found">Accumulate which <see cref="Opposite"/> pairs contain Side information</param>
        ///// <returns>Number of defined sides.</returns>
        //internal int CountSides(ref List<Pairs> found)
        //{
        //    int result = 0;
        //    if (found == null) { found = new List<Pairs>(); }

        //    if (!A.Side.Empty)
        //    {
        //        result += 1;
        //        found.Add(Pairs.A);
        //    }
        //    if (B.Side.Empty)
        //    {
        //        result += 1;
        //        found.Add(Pairs.B);
        //    }
        //    if (C.Side.Empty)
        //    {
        //        result += 1;
        //        found.Add(Pairs.C);
        //    }
        //    return result;
        //}

        ///// <summary>
        ///// Determines how many <see cref="Opposite"/> Pairs contain Angle information
        ///// </summary>
        ///// <param name="found">Accumulate which <see cref="Opposite"/> pairs contain Angle information</param>
        ///// <returns>Number of defined angles.</returns>
        //internal int CountAngles(ref List<Pairs> found)
        //{
        //    int result = 0;
        //    if (found == null) { found = new List<Pairs>(); }

        //    if (!A.Vertex.Empty)
        //    {
        //        result += 1;
        //        found.Add(Pairs.A);
        //    }
        //    if (!B.Vertex.Empty)
        //    {
        //        result += 1;
        //        found.Add(Pairs.B);
        //    }
        //    if (!C.Vertex.Empty)
        //    {
        //        result += 1;
        //        found.Add(Pairs.C);
        //    }
        //    return result;
        //}



        ///// <summary>
        ///// Returns the length of the side opposite the given <paramref name="angle"/>
        ///// </summary>
        ///// <param name="side1">Lenght of side 1</param>
        ///// <param name="angle">Angle in degrees between two sides</param>
        ///// <param name="side2">Length of side 2</param>
        ///// <returns></returns>
        //double SAS(double side1, double angle, double side2)
        //{
        //    //a = side1;
        //    //B = new Angle(angle);
        //    //c = side2;

        //    //double a2 = Math.Pow(a, 2);
        //    //double c2 = Math.Pow(c, 2);
        //    //return Math.Sqrt(a2 + c2 - 2 * a * c * Math.Cos(B.Radians));
        //    return 0;
        //}

        ///// <summary>
        ///// returns the length of side opposite to given <paramref name="angle2"/>
        ///// </summary>
        ///// <param name="angle1"></param>
        ///// <param name="angle2"></param>
        ///// <param name="side"></param>
        ///// <returns></returns>
        //double AAS(double angle1, double angle2, double side)
        //{
        //    //B = new Angle(angle1);
        //    //A = new Angle(angle2);
        //    //b = side;

        //    //return ((b * Math.Sin(A.Radians)) / Math.Sin(B.Radians));
        //    return 0;
        //}



    }




    //internal class LengthTrio
    //{
    //    readonly Rectangle Bounds;

    //    public double L1;

    //    public double L2;

    //    public double L3;

    //    // needs to use the bounds of rectangle to determine scale of lines, then increase/reduce all lines so they fit within the bounds
    //    public void Scale()
    //    {

    //    }
    //}


}





///// <summary>
///// Allow results to be returned with error message if required
///// </summary>
//internal class ResultMsg
//{
//    internal string Reason { get; }
//    internal bool Valid { get; set; }
//    internal bool Complete { get; set; }

//    /// <summary>
//    /// Define a complete, valid result
//    /// </summary>
//    internal ResultMsg()
//    {
//        Reason = string.Empty;
//        Valid = true;
//        Complete = true;
//    }

//    /// <summary>
//    /// Define an incomplete, valid result
//    /// </summary>
//    /// <param name="complete">Specify if result was completed</param>
//    internal ResultMsg(bool complete)
//    {
//        Reason = string.Empty;
//        Valid = true;
//        Complete = complete;
//    }

//    /// <summary>
//    /// Define invalid result with reason message
//    /// </summary>
//    /// <param name="reason">Specify the reason for the inavlid result</param>
//    internal ResultMsg(string reason)
//    {
//        Reason = reason;
//        Valid = false;
//        Complete = false;
//    }
//}

