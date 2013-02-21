using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarDataTools
{
    public enum XForm { Diff, VDiff, Copy, Set, Div, Mult, none};

    public class FeatureTransforms
    {
        private int _targetCol;

       
        private int[] _operandCols;

        
        private XForm _xformType = XForm.none;

        public int TargetCol
        {
            get { return _targetCol; }
            set { _targetCol = value; }
        }
        public int[] OperandCols
        {
            get { return _operandCols; }
            set { _operandCols = value; }
        }
        public XForm XformType
        {
            get { return _xformType; }
            set { _xformType = value; }
        }

        public FeatureTransforms(int targetCol, int operand0, int operand1, XForm xformType)
        {
            _targetCol = targetCol;
            _operandCols = new int[2];
            _operandCols[0] = operand0;
            _operandCols[1] = operand1;
            _xformType = xformType;

        }
        public FeatureTransforms(int targetCol, int operand0, int operand1, int operand2, XForm xformType)
        {
            _targetCol = targetCol;
            _operandCols = new int[3];
            _operandCols[0] = operand0;
            _operandCols[1] = operand1;
            _operandCols[2] = operand2;
            _xformType = xformType;

        }
        public void transform(double[,] xformData, double[,] features, long currentIndex)
        {
            switch (XformType)
            {
                case XForm.Diff:
                    xformData[currentIndex, _targetCol] = features[currentIndex, _operandCols[0]] - features[currentIndex, _operandCols[1]];
                    break;
                case XForm.VDiff:
                    long index2 = currentIndex - _operandCols[1];
                    if (index2 >= 0)
                    {
                        xformData[currentIndex, _targetCol] = features[currentIndex, _operandCols[0]] - features[index2, _operandCols[0]];
                    }
                    else
                    {
                        xformData[currentIndex, _targetCol] = 0;
                    }
                    break;
                case XForm.Copy:
                    xformData[currentIndex, _targetCol] = features[currentIndex, _operandCols[0]];
                    break;
                case XForm.Set:
                    xformData[currentIndex, _targetCol] = _operandCols[0];
                    break;
                case XForm.Div:
                    xformData[currentIndex, _targetCol] = features[currentIndex, _operandCols[0]]/_operandCols[1];
                    break;
                case XForm.Mult:
                    xformData[currentIndex, _targetCol] = features[currentIndex, _operandCols[0]] * _operandCols[1];
                    break;
            }
             
        }
    }
}
