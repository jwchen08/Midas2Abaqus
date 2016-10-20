using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas2Abaqus
{
    /*
     * Author: Syed Mehroz Alam
     * Email: smehrozalam@yahoo.com
     * URL: Programming Home "http://www.geocities.com/smehrozalam/" 
     * Date: 6/20/2004
     * Time: 4:46 PM
     *
     */

    /// <summary>
    /// Classes Contained:
    /// 	SimpleMatrix (version 1.1)
    /// 	SimpleMatrixException
    /// 	double (Version 2.0)
    /// 	doubleException
    /// </summary>


    /// Class name: SimpleMatrix
    /// Version: 1.1
    /// Class used: double
    /// Developed by: Syed Mehroz Alam
    /// Email: smehrozalam@yahoo.com
    /// URL: Programming Home "http://www.geocities.com/smehrozalam/"
    /// 
    /// What's New in version 1.1
    /// 	*	Added DeterminentFast() method
    /// 	*	Added InverseFast() method
    /// 	*	renamed ConvertToString to (override) ToString()
    /// 	*	some minor bugs fixed
    /// 
    /// Constructors:
    /// 	( double[,] ):	takes 2D doubles array	
    /// 	( int[,] ):	takes 2D integer array, convert them to doubles	
    /// 	( double[,] ):	takes 2D double array, convert them to doubles
    /// 	( int Rows, int Cols )	initializes the dimensions, indexers may be used 
    /// 							to set individual elements' values
    /// 
    /// Properties:
    /// 	Rows: read only property to get the no. of rows in the current SimpleMatrix
    /// 	Cols: read only property to get the no. of columns in the current SimpleMatrix
    /// 
    /// Indexers:
    /// 	[i,j] = used to set/get elements in the form of a double object
    /// 
    /// Public Methods (Description is given with respective methods' definitions)
    /// 	string ToString()
    /// 	SimpleMatrix Minor(SimpleMatrix, Row,Col)
    /// 	MultiplyRow( Row, double )
    /// 	MultiplyRow( Row, integer )
    /// 	MultiplyRow( Row, double )
    /// 	AddRow( TargetRow, SecondRow, Multiple)
    /// 	InterchangeRow( Row1, Row2)
    /// 	SimpleMatrix Concatenate(SimpleMatrix1, SimpleMatrix2)
    /// 	double Determinent()
    /// 	double DeterminentFast()
    /// 	SimpleMatrix EchelonForm()
    /// 	SimpleMatrix ReducedEchelonForm()
    /// 	SimpleMatrix Inverse()
    /// 	SimpleMatrix InverseFast()
    /// 	SimpleMatrix Adjoint()
    /// 	SimpleMatrix Transpose()
    /// 	SimpleMatrix Duplicate()
    /// 	SimpleMatrix ScalarSimpleMatrix( Rows, Cols, K )
    /// 	SimpleMatrix IdentitySimpleMatrix( Rows, Cols )
    /// 	SimpleMatrix UnitSimpleMatrix(Rows, Cols)
    /// 	SimpleMatrix NullSimpleMatrix(Rows, Cols)
    /// 
    /// Private Methods
    /// 	double GetElement(int iRow, int iCol)
    /// 	SetElement(int iRow, int iCol, double value)
    /// 	Negate(SimpleMatrix)
    /// 	Add(SimpleMatrix1, SimpleMatrix2)
    /// 	Multiply(SimpleMatrix1, SimpleMatrix2)
    /// 	Multiply(SimpleMatrix1, double)
    /// 	Multiply(SimpleMatrix1, integer)
    /// 
    /// Operators Overloaded Overloaded
    /// 	Unary: - (negate SimpleMatrix)
    /// 	Binary: 
    /// 		+,- for two matrices
    /// 		* for two matrices or one SimpleMatrix with integer or double or double
    /// 		/ for SimpleMatrix with integer or double or double
    /// </summary>
    public class SimpleMatrix
    {
        /// <summary>
        /// Class attributes/members
        /// </summary>
        int m_iRows;
        int m_iCols;
        double[,] m_iElement;


        /// <summary>
        /// Constructors
        /// </summary>
        public SimpleMatrix(double[,] elements)
        {
            m_iElement = elements;
            m_iRows = elements.GetLength(0);
            m_iCols = elements.GetLength(1);
        }

        public SimpleMatrix(int[,] elements)
        {
            m_iRows = elements.GetLength(0);
            m_iCols = elements.GetLength(1); ;
            m_iElement = new double[m_iRows, m_iCols];
            for (int i = 0; i < elements.GetLength(0); i++)
            {
                for (int j = 0; j < elements.GetLength(1); j++)
                {
                    this[i, j] = elements[i, j];
                }
            }
        }

        public SimpleMatrix(int iRows, int iCols)
        {
            m_iRows = iRows;
            m_iCols = iCols;
            m_iElement = new double[iRows, iCols];
        }

        /// <summary>
        /// Properites
        /// </summary>
        public int Rows
        {
            get { return m_iRows; }
        }

        public int Cols
        {
            get { return m_iCols; }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        public double this[int iRow, int iCol]		// SimpleMatrix's index starts at 0,0
        {
            get { return GetElement(iRow, iCol); }
            set { SetElement(iRow, iCol, value); }
        }

        /// <summary>
        /// Internal functions for getting/setting values
        /// </summary>
        private double GetElement(int iRow, int iCol)
        {
            if (iRow < 0 || iRow > Rows - 1 || iCol < 0 || iCol > Cols - 1)
                throw new SimpleMatrixException("Invalid index specified");
            return m_iElement[iRow, iCol];
        }

        private void SetElement(int iRow, int iCol, double value)
        {
            if (iRow < 0 || iRow > Rows - 1 || iCol < 0 || iCol > Cols - 1)
                throw new SimpleMatrixException("Invalid index specified");
            m_iElement[iRow, iCol] = value;
        }


        /// <summary>
        /// The function returns the current SimpleMatrix object as a string
        /// </summary>
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Cols; j++)
                    str += this[i, j] + "\t";
                str += "\n";
            }
            return str;
        }


        /// <summary>
        /// The function return the Minor of element[Row,Col] of a SimpleMatrix object 
        /// </summary>
        public static SimpleMatrix Minor(SimpleMatrix SimpleMatrix, int iRow, int iCol)
        {
            SimpleMatrix minor = new SimpleMatrix(SimpleMatrix.Rows - 1, SimpleMatrix.Cols - 1);
            int m = 0, n = 0;
            for (int i = 0; i < SimpleMatrix.Rows; i++)
            {
                if (i == iRow)
                    continue;
                n = 0;
                for (int j = 0; j < SimpleMatrix.Cols; j++)
                {
                    if (j == iCol)
                        continue;
                    minor[m, n] = SimpleMatrix[i, j];
                    n++;
                }
                m++;
            }
            return minor;
        }


        /// <summary>
        /// The function multiplies the given row of the current SimpleMatrix object by a double 
        /// </summary>
        public void MultiplyRow(int iRow, double frac)
        {
            for (int j = 0; j < this.Cols; j++)
            {
                this[iRow, j] *= frac;
            }
        }

        /// <summary>
        /// The function multiplies the given row of the current SimpleMatrix object by an integer
        /// </summary>
        public void MultiplyRow(int iRow, int iNo)
        {
            this.MultiplyRow(iRow, iNo);
        }

        /// <summary>
        /// The function adds two rows for current SimpleMatrix object
        /// It performs the following calculation:
        /// iTargetRow = iTargetRow + iMultiple*iSecondRow
        /// </summary>
        public void AddRow(int iTargetRow, int iSecondRow, double iMultiple)
        {
            for (int j = 0; j < this.Cols; j++)
                this[iTargetRow, j] += (this[iSecondRow, j] * iMultiple);
        }

        /// <summary>
        /// The function interchanges two rows of the current SimpleMatrix object
        /// </summary>
        public void InterchangeRow(int iRow1, int iRow2)
        {
            for (int j = 0; j < this.Cols; j++)
            {
                double temp = this[iRow1, j];
                this[iRow1, j] = this[iRow2, j];
                this[iRow2, j] = temp;
            }
        }

        /// <summary>
        /// The function concatenates the two given matrices column-wise
        /// it can be helpful in a equation solver class where the augmented SimpleMatrix is obtained by concatenation
        /// </summary>
        public static SimpleMatrix Concatenate(SimpleMatrix SimpleMatrix1, SimpleMatrix SimpleMatrix2)
        {
            if (SimpleMatrix1.Rows != SimpleMatrix2.Rows)
                throw new SimpleMatrixException("Concatenation not possible");
            SimpleMatrix SimpleMatrix = new SimpleMatrix(SimpleMatrix1.Rows, SimpleMatrix1.Cols + SimpleMatrix2.Cols);
            for (int i = 0; i < SimpleMatrix.Rows; i++)
                for (int j = 0; j < SimpleMatrix.Cols; j++)
                {
                    if (j < SimpleMatrix1.Cols)
                        SimpleMatrix[i, j] = SimpleMatrix1[i, j];
                    else
                        SimpleMatrix[i, j] = SimpleMatrix2[i, j - SimpleMatrix1.Cols];
                }
            return SimpleMatrix;
        }

        /// <summary>
        /// The function returns the determinent of the current SimpleMatrix object as double
        /// It computes the determinent by reducing the SimpleMatrix to reduced echelon form using row operations
        /// The function is very fast and efficient but may raise overflow exceptions in some cases.
        /// In such cases use the Determinent() function which computes determinent in the traditional 
        /// manner(by using minors)
        /// </summary>
        public double DeterminentFast()
        {
            if (this.Rows != this.Cols)
                throw new SimpleMatrixException("Determinent of a non-square SimpleMatrix doesn't exist");
            double det = 1;
            try
            {
                SimpleMatrix ReducedEchelonSimpleMatrix = this.Duplicate();
                for (int i = 0; i < this.Rows; i++)
                {
                    if (ReducedEchelonSimpleMatrix[i, i] == 0)	// if diagonal entry is zero, 
                        for (int j = i + 1; j < ReducedEchelonSimpleMatrix.Rows; j++)
                            if (ReducedEchelonSimpleMatrix[j, i] != 0)	 //check if some below entry is non-zero
                            {
                                ReducedEchelonSimpleMatrix.InterchangeRow(i, j);	// then interchange the two rows
                                det *= -1;	//interchanging two rows negates the determinent
                            }

                    det *= ReducedEchelonSimpleMatrix[i, i];
                    ReducedEchelonSimpleMatrix.MultiplyRow(i, (double)(1.0/(ReducedEchelonSimpleMatrix[i, i])));

                    for (int j = i + 1; j < ReducedEchelonSimpleMatrix.Rows; j++)
                    {
                        ReducedEchelonSimpleMatrix.AddRow(j, i, -ReducedEchelonSimpleMatrix[j, i]);
                    }
                    for (int j = i - 1; j >= 0; j--)
                    {
                        ReducedEchelonSimpleMatrix.AddRow(j, i, -ReducedEchelonSimpleMatrix[j, i]);
                    }
                }
                return det;
            }
            catch (Exception)
            {
                throw new SimpleMatrixException("Determinent of the given SimpleMatrix could not be calculated");
            }
        }

        /// <summary>
        /// The function returns the determinent of the current SimpleMatrix object as double
        /// It computes the determinent in the traditional way (i.e. using minors)
        /// It can be much slower(due to recursion) if the given SimpleMatrix has order greater than 6
        /// Try using DeterminentFast() function if the order of SimpleMatrix is greater than 6
        /// </summary>
        public double Determinent()
        {
            return Determinent(this);
        }

        /// <summary>
        /// The helper function for the above Determinent() method
        /// it calls itself recursively and computes determinent using minors
        /// </summary>
        private double Determinent(SimpleMatrix SimpleMatrix)
        {
            double det = 0;
            if (SimpleMatrix.Rows != SimpleMatrix.Cols)
                throw new SimpleMatrixException("Determinent of a non-square SimpleMatrix doesn't exist");
            if (SimpleMatrix.Rows == 1)
                return SimpleMatrix[0, 0];
            for (int j = 0; j < SimpleMatrix.Cols; j++)
                det += (SimpleMatrix[0, j] * Determinent(SimpleMatrix.Minor(SimpleMatrix, 0, j)) * (int)System.Math.Pow(-1, 0 + j));
            return det;
        }


        /// <summary>
        /// The function returns the Echelon form of the current SimpleMatrix
        /// </summary>
        public SimpleMatrix EchelonForm()
        {
            try
            {
                SimpleMatrix EchelonSimpleMatrix = this.Duplicate();
                for (int i = 0; i < this.Rows; i++)
                {
                    if (EchelonSimpleMatrix[i, i] == 0)	// if diagonal entry is zero, 
                        for (int j = i + 1; j < EchelonSimpleMatrix.Rows; j++)
                            if (EchelonSimpleMatrix[j, i] != 0)	 //check if some below entry is non-zero
                                EchelonSimpleMatrix.InterchangeRow(i, j);	// then interchange the two rows
                    if (EchelonSimpleMatrix[i, i] == 0)	// if not found any non-zero diagonal entry
                        continue;	// increment i;
                    if (EchelonSimpleMatrix[i, i] != 1)	// if diagonal entry is not 1 , 	
                        for (int j = i + 1; j < EchelonSimpleMatrix.Rows; j++)
                            if (EchelonSimpleMatrix[j, i] == 1)	 //check if some below entry is 1
                                EchelonSimpleMatrix.InterchangeRow(i, j);	// then interchange the two rows
                    EchelonSimpleMatrix.MultiplyRow(i, (double)(1.0/(EchelonSimpleMatrix[i, i])));
                    for (int j = i + 1; j < EchelonSimpleMatrix.Rows; j++)
                        EchelonSimpleMatrix.AddRow(j, i, -EchelonSimpleMatrix[j, i]);
                }
                return EchelonSimpleMatrix;
            }
            catch (Exception)
            {
                throw new SimpleMatrixException("SimpleMatrix can not be reduced to Echelon form");
            }
        }

        /// <summary>
        /// The function returns the reduced echelon form of the current SimpleMatrix
        /// </summary>
        public SimpleMatrix ReducedEchelonForm()
        {
            try
            {
                SimpleMatrix ReducedEchelonSimpleMatrix = this.Duplicate();
                for (int i = 0; i < this.Rows; i++)
                {
                    if (ReducedEchelonSimpleMatrix[i, i] == 0)	// if diagonal entry is zero, 
                        for (int j = i + 1; j < ReducedEchelonSimpleMatrix.Rows; j++)
                            if (ReducedEchelonSimpleMatrix[j, i] != 0)	 //check if some below entry is non-zero
                                ReducedEchelonSimpleMatrix.InterchangeRow(i, j);	// then interchange the two rows
                    if (ReducedEchelonSimpleMatrix[i, i] == 0)	// if not found any non-zero diagonal entry
                        continue;	// increment i;
                    if (ReducedEchelonSimpleMatrix[i, i] != 1)	// if diagonal entry is not 1 , 	
                        for (int j = i + 1; j < ReducedEchelonSimpleMatrix.Rows; j++)
                            if (ReducedEchelonSimpleMatrix[j, i] == 1)	 //check if some below entry is 1
                                ReducedEchelonSimpleMatrix.InterchangeRow(i, j);	// then interchange the two rows
                    ReducedEchelonSimpleMatrix.MultiplyRow(i, (double)(1.0/(ReducedEchelonSimpleMatrix[i, i])));
                    for (int j = i + 1; j < ReducedEchelonSimpleMatrix.Rows; j++)
                        ReducedEchelonSimpleMatrix.AddRow(j, i, -ReducedEchelonSimpleMatrix[j, i]);
                    for (int j = i - 1; j >= 0; j--)
                        ReducedEchelonSimpleMatrix.AddRow(j, i, -ReducedEchelonSimpleMatrix[j, i]);
                }
                return ReducedEchelonSimpleMatrix;
            }
            catch (Exception)
            {
                throw new SimpleMatrixException("SimpleMatrix can not be reduced to Echelon form");
            }
        }

        /// <summary>
        /// The function returns the inverse of the current SimpleMatrix using Reduced Echelon Form method
        /// The function is very fast and efficient but may raise overflow exceptions in some cases.
        /// In such cases use the Inverse() method which computes inverse in the traditional way(using adjoint).
        /// </summary>
        public SimpleMatrix InverseFast()
        {
            if (this.DeterminentFast() == 0)
                throw new SimpleMatrixException("Inverse of a singular SimpleMatrix is not possible");
            try
            {
                SimpleMatrix IdentitySimpleMatrix = SimpleMatrix.IdentitySimpleMatrix(this.Rows, this.Cols);
                SimpleMatrix ReducedEchelonSimpleMatrix = this.Duplicate();
                for (int i = 0; i < this.Rows; i++)
                {
                    if (ReducedEchelonSimpleMatrix[i, i] == 0)	// if diagonal entry is zero, 
                        for (int j = i + 1; j < ReducedEchelonSimpleMatrix.Rows; j++)
                            if (ReducedEchelonSimpleMatrix[j, i] != 0)	 //check if some below entry is non-zero
                            {
                                ReducedEchelonSimpleMatrix.InterchangeRow(i, j);	// then interchange the two rows
                                IdentitySimpleMatrix.InterchangeRow(i, j);	// then interchange the two rows
                            }
                    IdentitySimpleMatrix.MultiplyRow(i, (double)(1.0/(ReducedEchelonSimpleMatrix[i, i])));
                    ReducedEchelonSimpleMatrix.MultiplyRow(i, (double)(1.0/(ReducedEchelonSimpleMatrix[i, i])));

                    for (int j = i + 1; j < ReducedEchelonSimpleMatrix.Rows; j++)
                    {
                        IdentitySimpleMatrix.AddRow(j, i, -ReducedEchelonSimpleMatrix[j, i]);
                        ReducedEchelonSimpleMatrix.AddRow(j, i, -ReducedEchelonSimpleMatrix[j, i]);
                    }
                    for (int j = i - 1; j >= 0; j--)
                    {
                        IdentitySimpleMatrix.AddRow(j, i, -ReducedEchelonSimpleMatrix[j, i]);
                        ReducedEchelonSimpleMatrix.AddRow(j, i, -ReducedEchelonSimpleMatrix[j, i]);
                    }
                }
                return IdentitySimpleMatrix;
            }
            catch (Exception)
            {
                throw new SimpleMatrixException("Inverse of the given SimpleMatrix could not be calculated");
            }
        }

        /// <summary>
        /// The function returns the inverse of the current SimpleMatrix in the traditional way(by adjoint method)
        /// It can be much slower if the given SimpleMatrix has order greater than 6
        /// Try using InverseFast() function if the order of SimpleMatrix is greater than 6
        /// </summary>
        public SimpleMatrix Inverse()
        {
            if (this.Determinent() == 0)
                throw new SimpleMatrixException("Inverse of a singular SimpleMatrix is not possible");
            return (this.Adjoint() / this.Determinent());
        }

        /// <summary>
        /// The function returns the adjoint of the current SimpleMatrix
        /// </summary>
        public SimpleMatrix Adjoint()
        {
            if (this.Rows != this.Cols)
                throw new SimpleMatrixException("Adjoint of a non-square SimpleMatrix does not exists");
            SimpleMatrix AdjointSimpleMatrix = new SimpleMatrix(this.Rows, this.Cols);
            for (int i = 0; i < this.Rows; i++)
                for (int j = 0; j < this.Cols; j++)
                    AdjointSimpleMatrix[i, j] = Math.Pow(-1, i + j) * (Minor(this, i, j).Determinent());
            AdjointSimpleMatrix = AdjointSimpleMatrix.Transpose();
            return AdjointSimpleMatrix;
        }

        /// <summary>
        /// The function returns the transpose of the current SimpleMatrix
        /// </summary>
        public SimpleMatrix Transpose()
        {
            SimpleMatrix TransposeSimpleMatrix = new SimpleMatrix(this.Cols, this.Rows);
            for (int i = 0; i < TransposeSimpleMatrix.Rows; i++)
                for (int j = 0; j < TransposeSimpleMatrix.Cols; j++)
                    TransposeSimpleMatrix[i, j] = this[j, i];
            return TransposeSimpleMatrix;
        }

        /// <summary>
        /// The function duplicates the current SimpleMatrix object
        /// </summary>
        public SimpleMatrix Duplicate()
        {
            SimpleMatrix SimpleMatrix = new SimpleMatrix(Rows, Cols);
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    SimpleMatrix[i, j] = this[i, j];
            return SimpleMatrix;
        }

        /// <summary>
        /// The function returns a Scalar SimpleMatrix of dimension ( Row x Col ) and scalar K
        /// </summary>
        public static SimpleMatrix ScalarSimpleMatrix(int iRows, int iCols, int K)
        {
            double zero = 0;
            double scalar = K;
            SimpleMatrix SimpleMatrix = new SimpleMatrix(iRows, iCols);
            for (int i = 0; i < iRows; i++)
                for (int j = 0; j < iCols; j++)
                {
                    if (i == j)
                        SimpleMatrix[i, j] = scalar;
                    else
                        SimpleMatrix[i, j] = zero;
                }
            return SimpleMatrix;
        }

        /// <summary>
        /// The function returns an identity SimpleMatrix of dimensions ( Row x Col )
        /// </summary>
        public static SimpleMatrix IdentitySimpleMatrix(int iRows, int iCols)
        {
            return ScalarSimpleMatrix(iRows, iCols, 1);
        }

        /// <summary>
        /// The function returns a Unit SimpleMatrix of dimension ( Row x Col )
        /// </summary>
        public static SimpleMatrix UnitSimpleMatrix(int iRows, int iCols)
        {
            double temp = 1;
            SimpleMatrix SimpleMatrix = new SimpleMatrix(iRows, iCols);
            for (int i = 0; i < iRows; i++)
                for (int j = 0; j < iCols; j++)
                    SimpleMatrix[i, j] = temp;
            return SimpleMatrix;
        }

        /// <summary>
        /// The function returns a Null SimpleMatrix of dimension ( Row x Col )
        /// </summary>
        public static SimpleMatrix NullSimpleMatrix(int iRows, int iCols)
        {
            double temp = 0;
            SimpleMatrix SimpleMatrix = new SimpleMatrix(iRows, iCols);
            for (int i = 0; i < iRows; i++)
                for (int j = 0; j < iCols; j++)
                    SimpleMatrix[i, j] = temp;
            return SimpleMatrix;
        }

        /// <summary>
        /// Operators for the SimpleMatrix object
        /// includes -(unary), and binary opertors such as +,-,*,/
        /// </summary>
        public static SimpleMatrix operator -(SimpleMatrix SimpleMatrix)
        { return SimpleMatrix.Negate(SimpleMatrix); }

        public static SimpleMatrix operator +(SimpleMatrix SimpleMatrix1, SimpleMatrix SimpleMatrix2)
        { return SimpleMatrix.Add(SimpleMatrix1, SimpleMatrix2); }

        public static SimpleMatrix operator -(SimpleMatrix SimpleMatrix1, SimpleMatrix SimpleMatrix2)
        { return SimpleMatrix.Add(SimpleMatrix1, -SimpleMatrix2); }

        public static SimpleMatrix operator *(SimpleMatrix SimpleMatrix1, SimpleMatrix SimpleMatrix2)
        { return SimpleMatrix.Multiply(SimpleMatrix1, SimpleMatrix2); }

        public static SimpleMatrix operator *(SimpleMatrix SimpleMatrix1, int iNo)
        { return SimpleMatrix.Multiply(SimpleMatrix1, iNo); }

        public static SimpleMatrix operator *(SimpleMatrix SimpleMatrix1, double frac)
        { return SimpleMatrix.Multiply(SimpleMatrix1, frac); }

        public static SimpleMatrix operator *(int iNo, SimpleMatrix SimpleMatrix1)
        { return SimpleMatrix.Multiply(SimpleMatrix1, iNo); }

        public static SimpleMatrix operator *(double frac, SimpleMatrix SimpleMatrix1)
        { return SimpleMatrix.Multiply(SimpleMatrix1, frac); }

        public static SimpleMatrix operator /(SimpleMatrix SimpleMatrix1, int iNo)
        { return SimpleMatrix.Multiply(SimpleMatrix1, (double)(1.0/iNo)); }

        public static SimpleMatrix operator /(SimpleMatrix SimpleMatrix1, double frac)
        { return SimpleMatrix.Multiply(SimpleMatrix1, (double)(1.0/frac)); }


        /// <summary>
        /// Internal Fucntions for the above operators
        /// </summary>
        private static SimpleMatrix Negate(SimpleMatrix SimpleMatrix)
        {
            return SimpleMatrix.Multiply(SimpleMatrix, -1);
        }

        private static SimpleMatrix Add(SimpleMatrix SimpleMatrix1, SimpleMatrix SimpleMatrix2)
        {
            if (SimpleMatrix1.Rows != SimpleMatrix2.Rows || SimpleMatrix1.Cols != SimpleMatrix2.Cols)
                throw new SimpleMatrixException("Operation not possible");
            SimpleMatrix result = new SimpleMatrix(SimpleMatrix1.Rows, SimpleMatrix1.Cols);
            for (int i = 0; i < result.Rows; i++)
                for (int j = 0; j < result.Cols; j++)
                    result[i, j] = SimpleMatrix1[i, j] + SimpleMatrix2[i, j];
            return result;
        }

        private static SimpleMatrix Multiply(SimpleMatrix SimpleMatrix1, SimpleMatrix SimpleMatrix2)
        {
            if (SimpleMatrix1.Cols != SimpleMatrix2.Rows)
                throw new SimpleMatrixException("Operation not possible");
            SimpleMatrix result = SimpleMatrix.NullSimpleMatrix(SimpleMatrix1.Rows, SimpleMatrix2.Cols);
            for (int i = 0; i < result.Rows; i++)
                for (int j = 0; j < result.Cols; j++)
                    for (int k = 0; k < SimpleMatrix1.Cols; k++)
                        result[i, j] += SimpleMatrix1[i, k] * SimpleMatrix2[k, j];
            return result;
        }

        private static SimpleMatrix Multiply(SimpleMatrix SimpleMatrix, int iNo)
        {
            SimpleMatrix result = new SimpleMatrix(SimpleMatrix.Rows, SimpleMatrix.Cols);
            for (int i = 0; i < SimpleMatrix.Rows; i++)
                for (int j = 0; j < SimpleMatrix.Cols; j++)
                    result[i, j] = SimpleMatrix[i, j] * iNo;
            return result;
        }

        private static SimpleMatrix Multiply(SimpleMatrix SimpleMatrix, double frac)
        {
            SimpleMatrix result = new SimpleMatrix(SimpleMatrix.Rows, SimpleMatrix.Cols);
            for (int i = 0; i < SimpleMatrix.Rows; i++)
                for (int j = 0; j < SimpleMatrix.Cols; j++)
                    result[i, j] = SimpleMatrix[i, j] * frac;
            return result;
        }

    }	//end class SimpleMatrix

    /// <summary>
    /// Exception class for SimpleMatrix class, derived from System.Exception
    /// </summary>
    public class SimpleMatrixException : Exception
    {
        public SimpleMatrixException()
            : base()
        { }

        public SimpleMatrixException(string Message)
            : base(Message)
        { }

        public SimpleMatrixException(string Message, Exception InnerException)
            : base(Message, InnerException)
        { }
    }	// end class SimpleMatrixException



}
