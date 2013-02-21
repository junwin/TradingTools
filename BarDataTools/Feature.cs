using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HDF5DotNet;

namespace BarDataTools
{
    public class Feature
    {
        public void WriteFeaturesHDF5(double[,] featureSet, long[] winners)
        {
            try
            {
                // We will write and read an int array of this length.
                //const int DATA_ARRAY_ROWS = 12;
                //const int DATA_ARRAY_COLS = 5;

                // Rank is the number of dimensions of the data array.
                const int RANK = 2;

                // Create an HDF5 file.
                // The enumeration type H5F.CreateMode provides only the legal 
                // creation modes.  Missing H5Fcreate parameters are provided
                // with default values.
                H5FileId fileId = H5F.create("myCSharp1.hdf5", H5F.CreateMode.ACC_TRUNC);

                // Create a HDF5 group.  
                //H5GroupId groupId = H5G.create(fileId, "/cSharpGroup");
                //H5GroupId subGroup = H5G.create(groupId, "mySubGroup");

                // Close the subgroup.
                //H5G.close(subGroup);

                // Prepare to create a data space for writing a 2-dimensional 
                // signed integer array.
                long[] dims = new long[RANK];
                dims[0] = featureSet.GetLength(0);
                dims[1] = featureSet.GetLength(1);

                

                // Create a data space to accommodate our 1-dimensional array.
                // The resulting H5DataSpaceId will be used to create the 
                // data set.
                H5DataSpaceId spaceId = H5S.create_simple(RANK, dims);

                H5DataTypeId typeId = H5T.copy(H5T.H5Type.NATIVE_DOUBLE);

                // Find the size of the type
                int typeSize = H5T.getSize(H5T.H5Type.NATIVE_DOUBLE);
                

                // Set the order to big endian
                //H5T.setOrder(H5T.H5Type.NATIVE_DOUBLE, H5T.Order.BE);

                // Set the order to little endian
                H5T.setOrder(typeId, H5T.Order.LE);

                // Create the data set.
                H5DataSetId dataSetId = H5D.create(fileId, "/X", typeId, spaceId);

                // Write the integer data to the data set.
                H5D.write(dataSetId, new H5DataTypeId(H5T.H5Type.NATIVE_DOUBLE), new H5Array<double>(featureSet));

                long[] dims1 = new long[1];
                dims1[0] = winners.Length;

                double[] temp = new double[winners.Length];
                for (int i = 0; i < winners.Length; i++)
                {
                    temp[i] = winners[i];
                }
                H5DataTypeId typeId1 = H5T.copy(H5T.H5Type.NATIVE_DOUBLE);

                // Find the size of the type
                typeSize = H5T.getSize(H5T.H5Type.NATIVE_DOUBLE);

                // Set the order to little endian
                H5T.setOrder(typeId1, H5T.Order.LE);

                H5DataSpaceId spaceId1 = H5S.create_simple(1, dims1);
                // Create the data set.
                H5DataSetId dataSetId1 = H5D.create(fileId, "/y", typeId1, spaceId1);
                H5D.write(dataSetId1, new H5DataTypeId(H5T.H5Type.NATIVE_DOUBLE), new H5Array<double>(temp));
              
                // Close all the open resources.
                H5D.close(dataSetId);
                H5D.close(dataSetId1);
                H5T.close(typeId);
                H5T.close(typeId1);
                
                H5F.close(fileId);
            }
            // This catches all the HDF exception classes.  Because each call
            // generates a unique exception, different exception can be handled
            // separately.  For example, to catch open errors we could have used
            // catch (H5FopenException openException).
            catch (HDFException ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        private void CopyBarData(double[,] featureSet, K2DataObjects.PriceBar[] bars, long srcIndex, long targetIndex)
        {
            featureSet[targetIndex, 0] = (double)bars[srcIndex].TimeStamp;
            featureSet[targetIndex, 1] = (double)bars[srcIndex].ItemSize;
            featureSet[targetIndex, 2] = (double)bars[srcIndex].High;
            featureSet[targetIndex, 3] = (double)bars[srcIndex].Low;
            featureSet[targetIndex, 4] = (double)bars[srcIndex].Open;
            featureSet[targetIndex, 5] = (double)bars[srcIndex].Close;
            featureSet[targetIndex, 6] = (double)bars[srcIndex].Avg.Value;
            featureSet[targetIndex, 7] = (double)bars[srcIndex].Volume.Value;
            featureSet[targetIndex, 8] = (double)bars[srcIndex].BidVolume.Value;
            featureSet[targetIndex, 9] = (double)bars[srcIndex].AskVolume.Value;

        }
        private void CopyBarDataDelta(double[,] featureSet, K2DataObjects.PriceBar[] bars, long srcIndex, long targetIndex)
        {
            featureSet[targetIndex, 0] = (double)bars[srcIndex].TimeStamp;
            featureSet[targetIndex, 1] = (double)bars[srcIndex].ItemSize;
            if (srcIndex > 0)
            {
                featureSet[targetIndex, 2] = (double)bars[srcIndex].High - (double)bars[srcIndex - 1].High;
                featureSet[targetIndex, 3] = (double)bars[srcIndex].Low - (double)bars[srcIndex - 1].Low; 
                featureSet[targetIndex, 4] = (double)bars[srcIndex].Open - (double)bars[srcIndex - 1].Open;
                featureSet[targetIndex, 5] = (double)bars[srcIndex].Close - (double)bars[srcIndex - 1].Close;
                featureSet[targetIndex, 6] = (double)bars[srcIndex].Avg.Value - (double)bars[srcIndex - 1].Avg.Value;
                featureSet[targetIndex, 7] = (double)bars[srcIndex].Volume.Value - (double)bars[srcIndex - 1].Volume.Value;
                featureSet[targetIndex, 8] = (double)bars[srcIndex].BidVolume.Value - (double)bars[srcIndex - 1].BidVolume.Value;
                featureSet[targetIndex, 9] = (double)bars[srcIndex].AskVolume.Value - (double)bars[srcIndex - 1].AskVolume.Value;
            }
            else
            {
                featureSet[targetIndex, 2] = 0.0;
                featureSet[targetIndex, 3] = 0.0;
                featureSet[targetIndex, 4] = 0.0;
                featureSet[targetIndex, 5] = 0.0;
                featureSet[targetIndex, 6] = 0.0;
                featureSet[targetIndex, 7] = 0.0;
                featureSet[targetIndex, 8] = 0.0;
                featureSet[targetIndex, 9] = 0.0;
            }

        }
        private void CopyFeatureData(double[,] featureSet, decimal[][] curveValues, long srcIndex, long targetIndex, int offset)
        {
            for (int i = 0; i < curveValues[srcIndex].Length; i++)
            {
                featureSet[targetIndex, i + offset] = (double)curveValues[srcIndex][i];
            }
        }
        private void CopyFeatureDataDelta(double[,] featureSet, decimal[][] curveValues, long srcIndex, long targetIndex, int offset)
        {
            for (int i = 0; i < curveValues[srcIndex].Length; i++)
            {
                if (srcIndex > 0)
                {
                    featureSet[targetIndex, i + offset] = (double)curveValues[srcIndex][i] - (double)curveValues[srcIndex-1][i];
                }
                else
                {
                    featureSet[targetIndex, i + offset] = 0.0;
                }
            }
        }

        public double[,] ExtractCols(double[,] features, List<int> colIndexs)
        {
            double[,] newFeatures = new double[features.GetLength(0), colIndexs.Count];
            for (long i = 0; i < features.GetLength(0); i++)
            {
                for (int j = 0; j < colIndexs.Count; j++)
                {
                    newFeatures[i, j] = features[i, colIndexs[j]];
                }
            }
            return newFeatures;
        }

        public double[,] ApplyTransformsExtractCols(double[,] features, List<FeatureTransforms> transforms)
        {
            // scan for highest targetIndex
            int maxIndex = 0;
            foreach (FeatureTransforms f in transforms)
            {
                if (f.TargetCol > maxIndex)
                {
                    maxIndex = f.TargetCol;
                }
            }
            double[,] xformData = new double[features.GetLength(0), maxIndex+1];
            for (long i = 0; i < features.GetLength(0); i++)
            {
                foreach(FeatureTransforms f in transforms)
                {
                    f.transform(xformData, features, i);
                }
            }
            return xformData;
        }

        public void NormalizeFeatureSet(double[,] featureSet, long rows, long cols)
        {

            double[] maxVals = new double[cols];
            double[] minVals = new double[cols];
            double[] sizeFactor = new double[cols];
            for (long i = 0; i < cols; i++)
            {
                maxVals[i] = double.MinValue;
                minVals[i] = double.MaxValue;

            }
            for (long i = 0; i < rows; i++)
            {
                for (long j = 0; j < cols; j++)
                {
                    if (featureSet[i, j] != 0)
                    {
                        if (featureSet[i, j] > maxVals[j])
                        {
                            maxVals[j] = featureSet[i, j];
                        }
                        if (featureSet[i, j] < minVals[j])
                        {
                            minVals[j] = featureSet[i, j];
                        }
                    }
                }
            }
            for (long i = 0; i < cols; i++)
            {
                double diff = maxVals[i] - minVals[i];
                if (diff == 0)
                {
                    sizeFactor[i] = 1;
                }
                else
                {
                    sizeFactor[i] = diff;
                }

            }
            for (long i = 0; i < rows; i++)
            {
                for (long j = 0; j < cols; j++)
                {
                    featureSet[i, j] = (featureSet[i, j] - minVals[j]) / sizeFactor[j];

                }
            }
        }
        public int GetNumColumns(int numHeaders, int generations)
        {
            return numHeaders * (1 + generations) + 10;
        }

        public long CheckAlignment(K2DataObjects.PriceBar[] bars, decimal[][] curveValues, int count)
        {
            long position = 0;
            for (long i = 0; i < count; i++)
            {
                //if(bars[i].TimeStamp != curveValues[i], 
            }
            return 0;
        }
        public double[,] GenFeatureSet(K2DataObjects.PriceBar[] bars, decimal[][] curveValues, int count, List<string> headers, long generations)
        {
            // bars
            // move access to data out of here and pass in raw data
            int numCols = GetNumColumns(headers.Count, (int)generations);
            double[,] featureSet = new double[count, numCols];
            long index = 0;
            long row = 0;
            for (index = 0; index < curveValues.GetLength(0); index++)
            {
                
                if (generations <= index)
                {
                    CopyBarData(featureSet, bars, index, row);
                    for (int j = 0; j <= generations; j++)
                    {
                        int offset = 10 + (headers.Count * j);
                        CopyFeatureData(featureSet, curveValues, index - j, row, offset);
                    }
                    row++;
                    if (row >= count)
                    {                  
                        break;
                    }
                }
                
            }
            
            return featureSet;
        }

        public double[,] GenFeatureSetDelta(K2DataObjects.PriceBar[] bars, decimal[][] curveValues, int count, List<string> headers, long generations)
        {
            // bars
            // move access to data out of here and pass in raw data
            int numCols = GetNumColumns(headers.Count, (int)generations);
            double[,] featureSet = new double[count, numCols];
            long index = 0;
            long row = 0;
            for (index = 0; index < curveValues.GetLength(0); index++)
            {

                if (generations <= index)
                {
                    CopyBarDataDelta(featureSet, bars, index, row);
                    for (int j = 0; j <= generations; j++)
                    {
                        int offset = 10 + (headers.Count * j);
                        CopyFeatureDataDelta(featureSet, curveValues, index - j, row, offset);
                    }
                    row++;
                    if (row >= count)
                    {
                        break;
                    }
                }

            }

            return featureSet;
        }

        private string getDelimitedRow(int index, int numFeatures, double[,] features, double[] result, string delimiter)
        {
            string cdelData = "";
            for (int i = 0; i < numFeatures; i++)
            {
                cdelData += features[index, i].ToString() + delimiter;
            }
            cdelData += result[index].ToString();
            return cdelData;
        }
        private string getDelimitedRow(long index, int numFeatures, double[,] features, string delimiter, string format)
        {
            string cdelData = "";
            for (int i = 0; i < numFeatures; i++)
            {
                if (i < numFeatures - 1)
                {
                    cdelData += features[index, i].ToString(format) + delimiter;
                }
                else
                {
                    cdelData += features[index, i].ToString(format);
                }
            }

            return cdelData;
        }

        public void WriteFeaturesFile(string path, string name, double[,] features, long rows, long cols, string format)
        {
             FileStream fs = null;;
            StreamWriter sr = null;
            try
            {
                fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                sr = new StreamWriter(fs);
                Writefeatures(sr, name, features, rows, cols, format);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr = null;
                }

                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
            }
        }
        public void Writefeatures(StreamWriter sr, string name, double[,] features, long rows, long cols, string format)
        {

            try
            {


                sr.WriteLine("# Created by Octave 3.6.2, Thu Nov 29 21:33:46 2012 Central Standard Time <unknown@juwin7>");
                sr.WriteLine("# name: " + name);
                sr.WriteLine("# type: matrix");
                sr.WriteLine("# rows: " + rows.ToString());
                sr.WriteLine("# Columns: " + cols.ToString());

                if (format.Length == 0)
                {
                    format = "0.0000";
                }
                for (long i = 0; i < rows; i++)
                {
                    string rowData = getDelimitedRow(i, (int)cols, features, " ", format);
                    sr.WriteLine(rowData);
                }

                sr.Flush();

            }
            catch (Exception ex)
            {
            }
            

        }
        
      
    
    }
}
