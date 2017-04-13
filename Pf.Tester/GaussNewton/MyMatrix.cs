
// MyMatrix.cs
//
using System;

public class MyMatrix
{
    private int cols, rows;
    protected double[,] data;

    /// <summary>Construct a matrix</summary>
    public MyMatrix(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
        this.data = new double[rows, cols];
    }

    /// <summary>Construct a matrix by copying the data</summary>
    public MyMatrix(double[,] data) : this(data.GetLength(0), data.GetLength(1))
    {
        System.Buffer.BlockCopy(data, 0, this.data, 0, sizeof(double) * this.rows * this.cols);
    }

    /// <summary> Gets the number of rows in the matrix</summary>
    public int Rows { get { return this.rows; } }

    /// <summary> Gets the number of columns in the matrix</summary>
    public int Cols { get { return this.cols; } }

    /// <summary> Gets the raw data of the matrix. Should be treated as readonly</summary>
    public double[,] RawData { get { return this.data; } }

    /// <summary> Gets the sets the matrix element</summary>
    public double this[int row, int col]
    {
        get { return this.data[row, col]; }
        set { this.data[row, col] = value; }
    }

    /// <summary> Multiply by a vector. The width of the matrix should equal the height of the vector</summary>
    public double[] Multiply(double[] x)
    {
        if (x == null || x.Length != this.cols) throw new ArgumentException("Invalid vector");
        double[] result = new double[this.rows];

        for (int i = 0; i < result.Length; i++)
        {
            double sum = 0;
            for (int j = 0; j < this.cols; j++)
            {
                sum += this.data[i, j] * x[j];
            }
            result[i] = sum;
        }
        return result;
    }

    /// <summary> Multiply by another matrix. The width of left matrix should equal the height of right matrix </summary>
    public MyMatrix Multiply(MyMatrix m)
    {
        if (m == null || m.rows != this.cols) throw new ArgumentException("Invalid matrix to multiply");
        MyMatrix result = new MyMatrix(this.rows, m.cols);
        int inner = this.cols;

        for (int row = 0; row < result.rows; row++)
        {
            for (int col = 0; col < result.cols; col++)
            {
                double sum = 0;
                for (int i = 0; i < inner; i++) sum += this[row, i] * m[i, col];
                result[row, col] = sum;
            }
        }
        return result;
    }

    /// <summary> Create a transposed matrix </summary>
    public MyMatrix Transpose()
    {
        MyMatrix result = new MyMatrix(this.cols, this.rows);
        for (int row = 0; row < this.rows; row++)
        {
            for (int col = 0; col < this.cols; col++)
            {
                result[col, row] = this.data[row, col];
            }
        }
        return result;
    }

    public static double[] operator *(MyMatrix m, double[] vector)
    {
        return m.Multiply(vector);
    }

    public static MyMatrix operator *(MyMatrix m1, MyMatrix m2)
    {
        return m1.Multiply(m2);
    }
}

public class SquareMatrix : MyMatrix
{
    int rank;

    /// <summary>Construct an identity matrix</summary>
    public SquareMatrix(int rank) : base(rank, rank)
    {
        this.rank = rank;
        for (int i = 0; i < rank; i++) this.data[i, i] = 1;
    }

    /// <summary>Construct a square matrix by copying the data</summary>
    public SquareMatrix(double[,] data) : base(data)
    {
        if (data.GetLength(0) != data.GetLength(1))
        {
            throw new ArgumentException("Only square matrix supported");
        }
        this.rank = data.GetLength(0);
    }

    /// <summary>Use Laplace expansion to calculate the determinant of a square matrix</summary>
    public double Determinant()
    {
        if (this.rank == 1)
        {
            return data[0, 0];
        }
        if (this.rank == 2)
        {
            return data[0, 0] * data[1, 1] - data[1, 0] * data[0, 1];
        }

        double det = 0;
        for (int i = 0; i < this.rank; i++)
        {
            det += this.data[i, 0] * this.Cofactor(i, 0).Determinant() * (i % 2 == 0 ? 1 : -1);
        }
        return det;
    }

    /// <summary>Use Cramer's rule to recursively find the inverse of a square matrix</summary>
    public SquareMatrix Inverse()
    {
        double det = this.Determinant();
        if (Math.Abs(det) < 1e-8) throw new InvalidOperationException("Cannot inverse a singular matrix");

        SquareMatrix result = new SquareMatrix(this.rank);
        for (int i = 0; i < this.rank; i++)
        {
            for (int j = 0; j < this.rank; j++)
            {
                result.data[j, i] = this.Cofactor(i, j).Determinant() * ((i + j) % 2 == 0 ? 1 : -1) / det;
            }
        }
        return result;
    }

    /// <summary>Get a Cofactor matrix </summary>
    public SquareMatrix Cofactor(int i, int j)
    {
        if (this.rank < 2) throw new InvalidOperationException("Rank is less than two");

        SquareMatrix result = new SquareMatrix(this.rank - 1);
        double[,] buf = result.data;
        for (int y = 0; y < this.rank; y++)
        {
            for (int x = 0; x < this.rank; x++)
            {
                if (y != i && x != j)
                {
                    buf[y - (y > i ? 1 : 0), x - (x > j ? 1 : 0)] = this.data[y, x];
                }
            }
        }
        return result;
    }

    /// <summary> Create a square matrix from a general matrix</summary>
    public static SquareMatrix FromMatrix(MyMatrix m)
    {
        if (m == null || m.Cols != m.Rows) throw new ArgumentException("Not a valid square matrix");
        return new SquareMatrix(m.RawData);
    }
}