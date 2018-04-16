using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class P3Controller : MonoBehaviour
{
    public UnityEngine.UI.Text output;
    public string filename = "MAT340_MicahRust_P3";

    // Use this for initialization
    void Start()
    {
        // Input data pairs
        SetupData();
    }
    

    public void RunP3()
    {
        {
            //string path = Path.GetTempFileName();
            //using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Write, FileShare.None))
            //{
                string outText = "MAT 340a Summer 2018 Coding #3 Problem 3 Data File \nMicah Rust\n\n";
                
                PrintCalculation(ref outText, "(X)Midterm: <-> (Y)Course Grade", data_MTCG);
                PrintCalculation(ref outText, "(X)Homework <-> (Y)Course Grade", data_HWCG);
                PrintCalculation(ref outText, "(X)Quiz     <-> (Y)Course Grade", data_QCG);

                Byte[] info = new UTF8Encoding(true).GetBytes(outText);
                int byteOffset = info.Length;
                // Add the header information
                //Debug.Log("FileName is: " + fs.Name);
                
                //fs.Write(info, 0, info.Length);
                //fs.Close();

                Debug.Log(outText);
                output.text = outText;
            //}
        }
    }

    void PrintCalculation(ref string outText, string correlationName, Vector2[] data)
    {
        outText += correlationName + "\n";

        double meanX = 0f;
        double meanY = 0f;
        double stdevX = 0f;
        double stdevY = 0f;
        double correlationCoeficient = 0f;

        // Calculate Data
        {
            // mean + stdev
            double sumX = 0;
            double sumY = 0;
            double sumX_2 = 0;
            double sumY_2 = 0;
            foreach (var pair in data)
            {
                sumX += pair.x;
                sumY += pair.y;
                sumX_2 = pair.x * pair.x;
                sumY_2 = pair.y * pair.y;
            }
            // mean
            meanX = (sumX / (double)data.Length);
            meanY = (sumY / (double)data.Length);
            // stdev
            stdevX = (sumX_2 / (double)data.Length);
            stdevY = (sumY_2 / (double)data.Length);
            {
                // correlation coefficient
                double numeratorSum = 0;
                foreach (var pair in data) numeratorSum += (pair.x - meanX) * (pair.y - meanY);


                correlationCoeficient = numeratorSum / ((data.Length - 1) * stdevX * stdevY);
            }
        }
        
        outText += "Mean X: "  + meanX.ToString("#.###")  + "\n";
        outText += "Mean Y: " + meanY.ToString("#.###") + "\n";
        outText += "stdev X: " + stdevX.ToString("#.###") + "\n";
        outText += "stdev Y: " + stdevY.ToString("#.###") + "\n";
        outText += "Coefficient: " + correlationCoeficient.ToString("#.####") + "\n";

        outText += "\n\n";
    }
    


    Vector2[] data_MTCG;
    Vector2[] data_HWCG;
    Vector2[] data_QCG;
    private void SetupData()
    {
        const int count = 385;
        data_MTCG = new Vector2[count]; data_HWCG = new Vector2[count]; data_QCG = new Vector2[count];

        data_MTCG[0] = new Vector2(56f, 20f); data_HWCG[0] = new Vector2(25f, 20f); data_QCG[0] = new Vector2(24f, 20f);
        data_MTCG[1] = new Vector2(40f, 23f); data_HWCG[1] = new Vector2(16f, 23f); data_QCG[1] = new Vector2(24f, 23f);
        data_MTCG[2] = new Vector2(64f, 25f); data_HWCG[2] = new Vector2(27f, 25f); data_QCG[2] = new Vector2(19f, 25f);
        data_MTCG[3] = new Vector2(50f, 25f); data_HWCG[3] = new Vector2(30f, 25f); data_QCG[3] = new Vector2(29f, 25f);
        data_MTCG[4] = new Vector2(78f, 27.7f); data_HWCG[4] = new Vector2(29.5f, 27.7f); data_QCG[4] = new Vector2(45.5f, 27.7f);
        data_MTCG[5] = new Vector2(73f, 30f); data_HWCG[5] = new Vector2(39f, 30f); data_QCG[5] = new Vector2(32f, 30f);
        data_MTCG[6] = new Vector2(65f, 33f); data_HWCG[6] = new Vector2(46f, 33f); data_QCG[6] = new Vector2(58f, 33f);
        data_MTCG[7] = new Vector2(76f, 35f); data_HWCG[7] = new Vector2(85f, 35f); data_QCG[7] = new Vector2(41f, 35f);
        data_MTCG[8] = new Vector2(75f, 38f); data_HWCG[8] = new Vector2(53f, 38f); data_QCG[8] = new Vector2(36f, 38f);
        data_MTCG[9] = new Vector2(92f, 44f); data_HWCG[9] = new Vector2(37f, 44f); data_QCG[9] = new Vector2(74f, 44f);
        data_MTCG[10] = new Vector2(48f, 48f); data_HWCG[10] = new Vector2(96f, 48f); data_QCG[10] = new Vector2(39f, 48f);
        data_MTCG[11] = new Vector2(37f, 52f); data_HWCG[11] = new Vector2(23f, 52f); data_QCG[11] = new Vector2(41f, 52f);
        data_MTCG[12] = new Vector2(59f, 53f); data_HWCG[12] = new Vector2(66f, 53f); data_QCG[12] = new Vector2(51f, 53f);
        data_MTCG[13] = new Vector2(44f, 53f); data_HWCG[13] = new Vector2(93f, 53f); data_QCG[13] = new Vector2(46f, 53f);
        data_MTCG[14] = new Vector2(56f, 54f); data_HWCG[14] = new Vector2(35f, 54f); data_QCG[14] = new Vector2(60f, 54f);
        data_MTCG[15] = new Vector2(74f, 59f); data_HWCG[15] = new Vector2(53f, 59f); data_QCG[15] = new Vector2(59f, 59f);
        data_MTCG[16] = new Vector2(52f, 60f); data_HWCG[16] = new Vector2(90f, 60f); data_QCG[16] = new Vector2(49f, 60f);
        data_MTCG[17] = new Vector2(60f, 60f); data_HWCG[17] = new Vector2(27f, 60f); data_QCG[17] = new Vector2(61f, 60f);
        data_MTCG[18] = new Vector2(58f, 60f); data_HWCG[18] = new Vector2(28f, 60f); data_QCG[18] = new Vector2(46f, 60f);
        data_MTCG[19] = new Vector2(56f, 60.7f); data_HWCG[19] = new Vector2(41.1f, 60.7f); data_QCG[19] = new Vector2(51.5f, 60.7f);
        data_MTCG[20] = new Vector2(79f, 61f); data_HWCG[20] = new Vector2(64f, 61f); data_QCG[20] = new Vector2(60f, 61f);
        data_MTCG[21] = new Vector2(74f, 61f); data_HWCG[21] = new Vector2(47f, 61f); data_QCG[21] = new Vector2(50f, 61f);
        data_MTCG[22] = new Vector2(49f, 61.4f); data_HWCG[22] = new Vector2(85.3f, 61.4f); data_QCG[22] = new Vector2(75.8f, 61.4f);
        data_MTCG[23] = new Vector2(86f, 62f); data_HWCG[23] = new Vector2(85f, 62f); data_QCG[23] = new Vector2(68f, 62f);
        data_MTCG[24] = new Vector2(72f, 62f); data_HWCG[24] = new Vector2(46f, 62f); data_QCG[24] = new Vector2(63f, 62f);
        data_MTCG[25] = new Vector2(69f, 63f); data_HWCG[25] = new Vector2(100f, 63f); data_QCG[25] = new Vector2(49f, 63f);
        data_MTCG[26] = new Vector2(79f, 63f); data_HWCG[26] = new Vector2(26f, 63f); data_QCG[26] = new Vector2(58f, 63f);
        data_MTCG[27] = new Vector2(61f, 63f); data_HWCG[27] = new Vector2(56f, 63f); data_QCG[27] = new Vector2(36f, 63f);
        data_MTCG[28] = new Vector2(62f, 63.2f); data_HWCG[28] = new Vector2(90.1f, 63.2f); data_QCG[28] = new Vector2(69.7f, 63.2f);
        data_MTCG[29] = new Vector2(73f, 64f); data_HWCG[29] = new Vector2(36f, 64f); data_QCG[29] = new Vector2(70f, 64f);
        data_MTCG[30] = new Vector2(51f, 64f); data_HWCG[30] = new Vector2(52f, 64f); data_QCG[30] = new Vector2(50f, 64f);
        data_MTCG[31] = new Vector2(62f, 64f); data_HWCG[31] = new Vector2(74f, 64f); data_QCG[31] = new Vector2(58f, 64f);
        data_MTCG[32] = new Vector2(58f, 65f); data_HWCG[32] = new Vector2(73f, 65f); data_QCG[32] = new Vector2(61f, 65f);
        data_MTCG[33] = new Vector2(67f, 65f); data_HWCG[33] = new Vector2(64f, 65f); data_QCG[33] = new Vector2(63f, 65f);
        data_MTCG[34] = new Vector2(72f, 65f); data_HWCG[34] = new Vector2(71f, 65f); data_QCG[34] = new Vector2(72f, 65f);
        data_MTCG[35] = new Vector2(61f, 65f); data_HWCG[35] = new Vector2(69f, 65f); data_QCG[35] = new Vector2(58f, 65f);
        data_MTCG[36] = new Vector2(62f, 66f); data_HWCG[36] = new Vector2(91f, 66f); data_QCG[36] = new Vector2(62f, 66f);
        data_MTCG[37] = new Vector2(81f, 66f); data_HWCG[37] = new Vector2(27f, 66f); data_QCG[37] = new Vector2(70f, 66f);
        data_MTCG[38] = new Vector2(73f, 66f); data_HWCG[38] = new Vector2(30f, 66f); data_QCG[38] = new Vector2(63f, 66f);
        data_MTCG[39] = new Vector2(69f, 67f); data_HWCG[39] = new Vector2(48f, 67f); data_QCG[39] = new Vector2(70f, 67f);
        data_MTCG[40] = new Vector2(69f, 67f); data_HWCG[40] = new Vector2(43f, 67f); data_QCG[40] = new Vector2(58f, 67f);
        data_MTCG[41] = new Vector2(57f, 67f); data_HWCG[41] = new Vector2(97f, 67f); data_QCG[41] = new Vector2(53f, 67f);
        data_MTCG[42] = new Vector2(57f, 68f); data_HWCG[42] = new Vector2(76f, 68f); data_QCG[42] = new Vector2(59f, 68f);
        data_MTCG[43] = new Vector2(82f, 68f); data_HWCG[43] = new Vector2(9f, 68f); data_QCG[43] = new Vector2(44f, 68f);
        data_MTCG[44] = new Vector2(60f, 68f); data_HWCG[44] = new Vector2(66f, 68f); data_QCG[44] = new Vector2(90f, 68f);
        data_MTCG[45] = new Vector2(78f, 68f); data_HWCG[45] = new Vector2(64f, 68f); data_QCG[45] = new Vector2(80f, 68f);
        data_MTCG[46] = new Vector2(89f, 68f); data_HWCG[46] = new Vector2(37f, 68f); data_QCG[46] = new Vector2(44f, 68f);
        data_MTCG[47] = new Vector2(70f, 68.2f); data_HWCG[47] = new Vector2(90.4f, 68.2f); data_QCG[47] = new Vector2(86.4f, 68.2f);
        data_MTCG[48] = new Vector2(83f, 69f); data_HWCG[48] = new Vector2(23f, 69f); data_QCG[48] = new Vector2(79f, 69f);
        data_MTCG[49] = new Vector2(66f, 69f); data_HWCG[49] = new Vector2(81f, 69f); data_QCG[49] = new Vector2(45f, 69f);
        data_MTCG[50] = new Vector2(63f, 69f); data_HWCG[50] = new Vector2(82f, 69f); data_QCG[50] = new Vector2(70f, 69f);
        data_MTCG[51] = new Vector2(82f, 69f); data_HWCG[51] = new Vector2(57f, 69f); data_QCG[51] = new Vector2(99f, 69f);
        data_MTCG[52] = new Vector2(74f, 69f); data_HWCG[52] = new Vector2(45f, 69f); data_QCG[52] = new Vector2(61f, 69f);
        data_MTCG[53] = new Vector2(63f, 69f); data_HWCG[53] = new Vector2(86f, 69f); data_QCG[53] = new Vector2(70f, 69f);
        data_MTCG[54] = new Vector2(86f, 69f); data_HWCG[54] = new Vector2(33f, 69f); data_QCG[54] = new Vector2(70f, 69f);
        data_MTCG[55] = new Vector2(68f, 69f); data_HWCG[55] = new Vector2(65f, 69f); data_QCG[55] = new Vector2(68f, 69f);
        data_MTCG[56] = new Vector2(61f, 69.6f); data_HWCG[56] = new Vector2(73.2f, 69.6f); data_QCG[56] = new Vector2(71.2f, 69.6f);
        data_MTCG[57] = new Vector2(69f, 70f); data_HWCG[57] = new Vector2(64f, 70f); data_QCG[57] = new Vector2(70f, 70f);
        data_MTCG[58] = new Vector2(83f, 70f); data_HWCG[58] = new Vector2(57f, 70f); data_QCG[58] = new Vector2(79f, 70f);
        data_MTCG[59] = new Vector2(76f, 70f); data_HWCG[59] = new Vector2(60f, 70f); data_QCG[59] = new Vector2(69f, 70f);
        data_MTCG[60] = new Vector2(67f, 70f); data_HWCG[60] = new Vector2(83f, 70f); data_QCG[60] = new Vector2(81f, 70f);
        data_MTCG[61] = new Vector2(74f, 70.4f); data_HWCG[61] = new Vector2(87.9f, 70.4f); data_QCG[61] = new Vector2(66.7f, 70.4f);
        data_MTCG[62] = new Vector2(87f, 70.9f); data_HWCG[62] = new Vector2(87.9f, 70.9f); data_QCG[62] = new Vector2(80.3f, 70.9f);
        data_MTCG[63] = new Vector2(63f, 71f); data_HWCG[63] = new Vector2(92f, 71f); data_QCG[63] = new Vector2(54f, 71f);
        data_MTCG[64] = new Vector2(72f, 71f); data_HWCG[64] = new Vector2(82f, 71f); data_QCG[64] = new Vector2(56f, 71f);
        data_MTCG[65] = new Vector2(73f, 71f); data_HWCG[65] = new Vector2(88f, 71f); data_QCG[65] = new Vector2(73f, 71f);
        data_MTCG[66] = new Vector2(79f, 71f); data_HWCG[66] = new Vector2(15f, 71f); data_QCG[66] = new Vector2(63f, 71f);
        data_MTCG[67] = new Vector2(74f, 71f); data_HWCG[67] = new Vector2(79f, 71f); data_QCG[67] = new Vector2(68f, 71f);
        data_MTCG[68] = new Vector2(71f, 71f); data_HWCG[68] = new Vector2(80f, 71f); data_QCG[68] = new Vector2(66f, 71f);
        data_MTCG[69] = new Vector2(89f, 71f); data_HWCG[69] = new Vector2(87.9f, 71f); data_QCG[69] = new Vector2(28.8f, 71f);
        data_MTCG[70] = new Vector2(64f, 72f); data_HWCG[70] = new Vector2(99f, 72f); data_QCG[70] = new Vector2(56f, 72f);
        data_MTCG[71] = new Vector2(64f, 72f); data_HWCG[71] = new Vector2(92f, 72f); data_QCG[71] = new Vector2(73f, 72f);
        data_MTCG[72] = new Vector2(60f, 72f); data_HWCG[72] = new Vector2(57f, 72f); data_QCG[72] = new Vector2(72f, 72f);
        data_MTCG[73] = new Vector2(61f, 72f); data_HWCG[73] = new Vector2(78f, 72f); data_QCG[73] = new Vector2(74f, 72f);
        data_MTCG[74] = new Vector2(75f, 72f); data_HWCG[74] = new Vector2(82f, 72f); data_QCG[74] = new Vector2(74f, 72f);
        data_MTCG[75] = new Vector2(66f, 72f); data_HWCG[75] = new Vector2(43f, 72f); data_QCG[75] = new Vector2(70f, 72f);
        data_MTCG[76] = new Vector2(73f, 72f); data_HWCG[76] = new Vector2(87f, 72f); data_QCG[76] = new Vector2(71f, 72f);
        data_MTCG[77] = new Vector2(76f, 72f); data_HWCG[77] = new Vector2(64f, 72f); data_QCG[77] = new Vector2(71f, 72f);
        data_MTCG[78] = new Vector2(67f, 72f); data_HWCG[78] = new Vector2(96f, 72f); data_QCG[78] = new Vector2(71f, 72f);
        data_MTCG[79] = new Vector2(78f, 72f); data_HWCG[79] = new Vector2(40f, 72f); data_QCG[79] = new Vector2(43f, 72f);
        data_MTCG[80] = new Vector2(71f, 72f); data_HWCG[80] = new Vector2(76.7f, 72f); data_QCG[80] = new Vector2(78.8f, 72f);
        data_MTCG[81] = new Vector2(71f, 72.9f); data_HWCG[81] = new Vector2(82.5f, 72.9f); data_QCG[81] = new Vector2(77.3f, 72.9f);
        data_MTCG[82] = new Vector2(68f, 73f); data_HWCG[82] = new Vector2(97f, 73f); data_QCG[82] = new Vector2(64f, 73f);
        data_MTCG[83] = new Vector2(69f, 73f); data_HWCG[83] = new Vector2(73f, 73f); data_QCG[83] = new Vector2(73f, 73f);
        data_MTCG[84] = new Vector2(87f, 73f); data_HWCG[84] = new Vector2(23f, 73f); data_QCG[84] = new Vector2(78f, 73f);
        data_MTCG[85] = new Vector2(73f, 73f); data_HWCG[85] = new Vector2(69f, 73f); data_QCG[85] = new Vector2(56f, 73f);
        data_MTCG[86] = new Vector2(59f, 73f); data_HWCG[86] = new Vector2(88f, 73f); data_QCG[86] = new Vector2(70f, 73f);
        data_MTCG[87] = new Vector2(67f, 73f); data_HWCG[87] = new Vector2(84f, 73f); data_QCG[87] = new Vector2(71f, 73f);
        data_MTCG[88] = new Vector2(73f, 73f); data_HWCG[88] = new Vector2(70f, 73f); data_QCG[88] = new Vector2(69f, 73f);
        data_MTCG[89] = new Vector2(63f, 73f); data_HWCG[89] = new Vector2(90f, 73f); data_QCG[89] = new Vector2(53f, 73f);
        data_MTCG[90] = new Vector2(72f, 73f); data_HWCG[90] = new Vector2(77f, 73f); data_QCG[90] = new Vector2(74f, 73f);
        data_MTCG[91] = new Vector2(64f, 73f); data_HWCG[91] = new Vector2(53f, 73f); data_QCG[91] = new Vector2(74f, 73f);
        data_MTCG[92] = new Vector2(61f, 73f); data_HWCG[92] = new Vector2(91f, 73f); data_QCG[92] = new Vector2(70f, 73f);
        data_MTCG[93] = new Vector2(72f, 73f); data_HWCG[93] = new Vector2(95f, 73f); data_QCG[93] = new Vector2(73f, 73f);
        data_MTCG[94] = new Vector2(70f, 73f); data_HWCG[94] = new Vector2(69f, 73f); data_QCG[94] = new Vector2(61f, 73f);
        data_MTCG[95] = new Vector2(85f, 73.3f); data_HWCG[95] = new Vector2(90.6f, 73.3f); data_QCG[95] = new Vector2(77.3f, 73.3f);
        data_MTCG[96] = new Vector2(83f, 73.5f); data_HWCG[96] = new Vector2(87.7f, 73.5f); data_QCG[96] = new Vector2(59.1f, 73.5f);
        data_MTCG[97] = new Vector2(59f, 74f); data_HWCG[97] = new Vector2(93f, 74f); data_QCG[97] = new Vector2(77f, 74f);
        data_MTCG[98] = new Vector2(78f, 74f); data_HWCG[98] = new Vector2(23f, 74f); data_QCG[98] = new Vector2(67f, 74f);
        data_MTCG[99] = new Vector2(77f, 74f); data_HWCG[99] = new Vector2(80f, 74f); data_QCG[99] = new Vector2(60f, 74f);
        data_MTCG[100] = new Vector2(84f, 74f); data_HWCG[100] = new Vector2(86f, 74f); data_QCG[100] = new Vector2(68f, 74f);
        data_MTCG[101] = new Vector2(76f, 74f); data_HWCG[101] = new Vector2(91f, 74f); data_QCG[101] = new Vector2(60f, 74f);
        data_MTCG[102] = new Vector2(88f, 74f); data_HWCG[102] = new Vector2(81f, 74f); data_QCG[102] = new Vector2(60f, 74f);
        data_MTCG[103] = new Vector2(77f, 74f); data_HWCG[103] = new Vector2(74f, 74f); data_QCG[103] = new Vector2(80f, 74f);
        data_MTCG[104] = new Vector2(67f, 74f); data_HWCG[104] = new Vector2(65f, 74f); data_QCG[104] = new Vector2(72f, 74f);
        data_MTCG[105] = new Vector2(75f, 74f); data_HWCG[105] = new Vector2(90f, 74f); data_QCG[105] = new Vector2(81f, 74f);
        data_MTCG[106] = new Vector2(80f, 74f); data_HWCG[106] = new Vector2(71f, 74f); data_QCG[106] = new Vector2(83f, 74f);
        data_MTCG[107] = new Vector2(79f, 75f); data_HWCG[107] = new Vector2(61f, 75f); data_QCG[107] = new Vector2(66f, 75f);
        data_MTCG[108] = new Vector2(66f, 75f); data_HWCG[108] = new Vector2(92f, 75f); data_QCG[108] = new Vector2(74f, 75f);
        data_MTCG[109] = new Vector2(67f, 75f); data_HWCG[109] = new Vector2(86f, 75f); data_QCG[109] = new Vector2(75f, 75f);
        data_MTCG[110] = new Vector2(76f, 75f); data_HWCG[110] = new Vector2(49f, 75f); data_QCG[110] = new Vector2(65f, 75f);
        data_MTCG[111] = new Vector2(72f, 75f); data_HWCG[111] = new Vector2(87f, 75f); data_QCG[111] = new Vector2(81f, 75f);
        data_MTCG[112] = new Vector2(85f, 75f); data_HWCG[112] = new Vector2(75f, 75f); data_QCG[112] = new Vector2(49f, 75f);
        data_MTCG[113] = new Vector2(63f, 75f); data_HWCG[113] = new Vector2(82f, 75f); data_QCG[113] = new Vector2(72f, 75f);
        data_MTCG[114] = new Vector2(80f, 75.9f); data_HWCG[114] = new Vector2(67f, 75.9f); data_QCG[114] = new Vector2(75.8f, 75.9f);
        data_MTCG[115] = new Vector2(73f, 76f); data_HWCG[115] = new Vector2(86f, 76f); data_QCG[115] = new Vector2(83f, 76f);
        data_MTCG[116] = new Vector2(65f, 76f); data_HWCG[116] = new Vector2(94f, 76f); data_QCG[116] = new Vector2(77f, 76f);
        data_MTCG[117] = new Vector2(67f, 76f); data_HWCG[117] = new Vector2(87f, 76f); data_QCG[117] = new Vector2(81f, 76f);
        data_MTCG[118] = new Vector2(71f, 76f); data_HWCG[118] = new Vector2(85f, 76f); data_QCG[118] = new Vector2(63f, 76f);
        data_MTCG[119] = new Vector2(71f, 76f); data_HWCG[119] = new Vector2(95f, 76f); data_QCG[119] = new Vector2(74f, 76f);
        data_MTCG[120] = new Vector2(77f, 76f); data_HWCG[120] = new Vector2(88f, 76f); data_QCG[120] = new Vector2(84f, 76f);
        data_MTCG[121] = new Vector2(58f, 76f); data_HWCG[121] = new Vector2(85f, 76f); data_QCG[121] = new Vector2(82f, 76f);
        data_MTCG[122] = new Vector2(88f, 76f); data_HWCG[122] = new Vector2(86f, 76f); data_QCG[122] = new Vector2(78f, 76f);
        data_MTCG[123] = new Vector2(79f, 76f); data_HWCG[123] = new Vector2(92f, 76f); data_QCG[123] = new Vector2(64f, 76f);
        data_MTCG[124] = new Vector2(83f, 76f); data_HWCG[124] = new Vector2(38f, 76f); data_QCG[124] = new Vector2(69f, 76f);
        data_MTCG[125] = new Vector2(83f, 76.2f); data_HWCG[125] = new Vector2(94.6f, 76.2f); data_QCG[125] = new Vector2(81.8f, 76.2f);
        data_MTCG[126] = new Vector2(81f, 76.2f); data_HWCG[126] = new Vector2(78.3f, 76.2f); data_QCG[126] = new Vector2(74.2f, 76.2f);
        data_MTCG[127] = new Vector2(87f, 76.6f); data_HWCG[127] = new Vector2(78.5f, 76.6f); data_QCG[127] = new Vector2(78.8f, 76.6f);
        data_MTCG[128] = new Vector2(66f, 76.7f); data_HWCG[128] = new Vector2(98.9f, 76.7f); data_QCG[128] = new Vector2(81.8f, 76.7f);
        data_MTCG[129] = new Vector2(78f, 76.7f); data_HWCG[129] = new Vector2(84.6f, 76.7f); data_QCG[129] = new Vector2(72.7f, 76.7f);
        data_MTCG[130] = new Vector2(58f, 76.7f); data_HWCG[130] = new Vector2(100f, 76.7f); data_QCG[130] = new Vector2(86.4f, 76.7f);
        data_MTCG[131] = new Vector2(74f, 77f); data_HWCG[131] = new Vector2(76f, 77f); data_QCG[131] = new Vector2(80f, 77f);
        data_MTCG[132] = new Vector2(68f, 77f); data_HWCG[132] = new Vector2(83f, 77f); data_QCG[132] = new Vector2(80f, 77f);
        data_MTCG[133] = new Vector2(59f, 77f); data_HWCG[133] = new Vector2(87f, 77f); data_QCG[133] = new Vector2(82f, 77f);
        data_MTCG[134] = new Vector2(73f, 77f); data_HWCG[134] = new Vector2(79f, 77f); data_QCG[134] = new Vector2(83f, 77f);
        data_MTCG[135] = new Vector2(75f, 77f); data_HWCG[135] = new Vector2(96f, 77f); data_QCG[135] = new Vector2(64f, 77f);
        data_MTCG[136] = new Vector2(84f, 77f); data_HWCG[136] = new Vector2(52f, 77f); data_QCG[136] = new Vector2(78f, 77f);
        data_MTCG[137] = new Vector2(78f, 77f); data_HWCG[137] = new Vector2(71f, 77f); data_QCG[137] = new Vector2(89f, 77f);
        data_MTCG[138] = new Vector2(69f, 77f); data_HWCG[138] = new Vector2(86f, 77f); data_QCG[138] = new Vector2(84f, 77f);
        data_MTCG[139] = new Vector2(70f, 77f); data_HWCG[139] = new Vector2(84f, 77f); data_QCG[139] = new Vector2(82f, 77f);
        data_MTCG[140] = new Vector2(74f, 77f); data_HWCG[140] = new Vector2(84f, 77f); data_QCG[140] = new Vector2(76f, 77f);
        data_MTCG[141] = new Vector2(70f, 77f); data_HWCG[141] = new Vector2(87f, 77f); data_QCG[141] = new Vector2(78f, 77f);
        data_MTCG[142] = new Vector2(87f, 77.4f); data_HWCG[142] = new Vector2(96.5f, 77.4f); data_QCG[142] = new Vector2(89.4f, 77.4f);
        data_MTCG[143] = new Vector2(80f, 78f); data_HWCG[143] = new Vector2(67f, 78f); data_QCG[143] = new Vector2(80f, 78f);
        data_MTCG[144] = new Vector2(79f, 78f); data_HWCG[144] = new Vector2(87f, 78f); data_QCG[144] = new Vector2(96f, 78f);
        data_MTCG[145] = new Vector2(66f, 78f); data_HWCG[145] = new Vector2(83f, 78f); data_QCG[145] = new Vector2(76f, 78f);
        data_MTCG[146] = new Vector2(72f, 78f); data_HWCG[146] = new Vector2(98f, 78f); data_QCG[146] = new Vector2(72f, 78f);
        data_MTCG[147] = new Vector2(69f, 78f); data_HWCG[147] = new Vector2(95f, 78f); data_QCG[147] = new Vector2(73f, 78f);
        data_MTCG[148] = new Vector2(75f, 78f); data_HWCG[148] = new Vector2(89f, 78f); data_QCG[148] = new Vector2(84f, 78f);
        data_MTCG[149] = new Vector2(76f, 78f); data_HWCG[149] = new Vector2(92f, 78f); data_QCG[149] = new Vector2(91f, 78f);
        data_MTCG[150] = new Vector2(76f, 78f); data_HWCG[150] = new Vector2(79f, 78f); data_QCG[150] = new Vector2(84f, 78f);
        data_MTCG[151] = new Vector2(77f, 78f); data_HWCG[151] = new Vector2(61f, 78f); data_QCG[151] = new Vector2(64f, 78f);
        data_MTCG[152] = new Vector2(60f, 78f); data_HWCG[152] = new Vector2(73f, 78f); data_QCG[152] = new Vector2(81f, 78f);
        data_MTCG[153] = new Vector2(73f, 78f); data_HWCG[153] = new Vector2(75f, 78f); data_QCG[153] = new Vector2(90f, 78f);
        data_MTCG[154] = new Vector2(78f, 78f); data_HWCG[154] = new Vector2(88f, 78f); data_QCG[154] = new Vector2(86f, 78f);
        data_MTCG[155] = new Vector2(70f, 79f); data_HWCG[155] = new Vector2(85f, 79f); data_QCG[155] = new Vector2(71f, 79f);
        data_MTCG[156] = new Vector2(68f, 79f); data_HWCG[156] = new Vector2(86f, 79f); data_QCG[156] = new Vector2(83f, 79f);
        data_MTCG[157] = new Vector2(79f, 79f); data_HWCG[157] = new Vector2(96f, 79f); data_QCG[157] = new Vector2(76f, 79f);
        data_MTCG[158] = new Vector2(75f, 79f); data_HWCG[158] = new Vector2(85f, 79f); data_QCG[158] = new Vector2(73f, 79f);
        data_MTCG[159] = new Vector2(85f, 79f); data_HWCG[159] = new Vector2(80f, 79f); data_QCG[159] = new Vector2(65f, 79f);
        data_MTCG[160] = new Vector2(73f, 79f); data_HWCG[160] = new Vector2(85f, 79f); data_QCG[160] = new Vector2(84f, 79f);
        data_MTCG[161] = new Vector2(86f, 79f); data_HWCG[161] = new Vector2(86f, 79f); data_QCG[161] = new Vector2(89f, 79f);
        data_MTCG[162] = new Vector2(76f, 79f); data_HWCG[162] = new Vector2(80f, 79f); data_QCG[162] = new Vector2(72f, 79f);
        data_MTCG[163] = new Vector2(78f, 80f); data_HWCG[163] = new Vector2(67f, 80f); data_QCG[163] = new Vector2(50f, 80f);
        data_MTCG[164] = new Vector2(67f, 80f); data_HWCG[164] = new Vector2(76f, 80f); data_QCG[164] = new Vector2(86f, 80f);
        data_MTCG[165] = new Vector2(78f, 80f); data_HWCG[165] = new Vector2(90f, 80f); data_QCG[165] = new Vector2(74f, 80f);
        data_MTCG[166] = new Vector2(64f, 80f); data_HWCG[166] = new Vector2(100f, 80f); data_QCG[166] = new Vector2(76f, 80f);
        data_MTCG[167] = new Vector2(79f, 80f); data_HWCG[167] = new Vector2(87f, 80f); data_QCG[167] = new Vector2(87f, 80f);
        data_MTCG[168] = new Vector2(98f, 80f); data_HWCG[168] = new Vector2(86f, 80f); data_QCG[168] = new Vector2(89f, 80f);
        data_MTCG[169] = new Vector2(84f, 80f); data_HWCG[169] = new Vector2(64f, 80f); data_QCG[169] = new Vector2(86f, 80f);
        data_MTCG[170] = new Vector2(92f, 80f); data_HWCG[170] = new Vector2(78f, 80f); data_QCG[170] = new Vector2(80f, 80f);
        data_MTCG[171] = new Vector2(79f, 80f); data_HWCG[171] = new Vector2(83f, 80f); data_QCG[171] = new Vector2(75f, 80f);
        data_MTCG[172] = new Vector2(84f, 80f); data_HWCG[172] = new Vector2(82f, 80f); data_QCG[172] = new Vector2(76f, 80f);
        data_MTCG[173] = new Vector2(72f, 80f); data_HWCG[173] = new Vector2(100f, 80f); data_QCG[173] = new Vector2(76f, 80f);
        data_MTCG[174] = new Vector2(84f, 80f); data_HWCG[174] = new Vector2(91f, 80f); data_QCG[174] = new Vector2(86f, 80f);
        data_MTCG[175] = new Vector2(78f, 80f); data_HWCG[175] = new Vector2(86f, 80f); data_QCG[175] = new Vector2(86f, 80f);
        data_MTCG[176] = new Vector2(77f, 80f); data_HWCG[176] = new Vector2(75f, 80f); data_QCG[176] = new Vector2(83f, 80f);
        data_MTCG[177] = new Vector2(73f, 80.3f); data_HWCG[177] = new Vector2(87.4f, 80.3f); data_QCG[177] = new Vector2(78.8f, 80.3f);
        data_MTCG[178] = new Vector2(89f, 80.5f); data_HWCG[178] = new Vector2(90.8f, 80.5f); data_QCG[178] = new Vector2(83.3f, 80.5f);
        data_MTCG[179] = new Vector2(89f, 80.7f); data_HWCG[179] = new Vector2(86.7f, 80.7f); data_QCG[179] = new Vector2(80.3f, 80.7f);
        data_MTCG[180] = new Vector2(77f, 81f); data_HWCG[180] = new Vector2(94f, 81f); data_QCG[180] = new Vector2(69f, 81f);
        data_MTCG[181] = new Vector2(98f, 81f); data_HWCG[181] = new Vector2(74f, 81f); data_QCG[181] = new Vector2(77f, 81f);
        data_MTCG[182] = new Vector2(90f, 81f); data_HWCG[182] = new Vector2(97f, 81f); data_QCG[182] = new Vector2(76f, 81f);
        data_MTCG[183] = new Vector2(80f, 81f); data_HWCG[183] = new Vector2(38f, 81f); data_QCG[183] = new Vector2(88f, 81f);
        data_MTCG[184] = new Vector2(74f, 81f); data_HWCG[184] = new Vector2(91f, 81f); data_QCG[184] = new Vector2(80f, 81f);
        data_MTCG[185] = new Vector2(87f, 81f); data_HWCG[185] = new Vector2(45f, 81f); data_QCG[185] = new Vector2(87f, 81f);
        data_MTCG[186] = new Vector2(70f, 81f); data_HWCG[186] = new Vector2(94f, 81f); data_QCG[186] = new Vector2(78f, 81f);
        data_MTCG[187] = new Vector2(80f, 81.5f); data_HWCG[187] = new Vector2(86.7f, 81.5f); data_QCG[187] = new Vector2(89.4f, 81.5f);
        data_MTCG[188] = new Vector2(89f, 81.9f); data_HWCG[188] = new Vector2(99f, 81.9f); data_QCG[188] = new Vector2(75.8f, 81.9f);
        data_MTCG[189] = new Vector2(87f, 82f); data_HWCG[189] = new Vector2(90f, 82f); data_QCG[189] = new Vector2(64f, 82f);
        data_MTCG[190] = new Vector2(85f, 82f); data_HWCG[190] = new Vector2(93f, 82f); data_QCG[190] = new Vector2(81f, 82f);
        data_MTCG[191] = new Vector2(83f, 82f); data_HWCG[191] = new Vector2(46f, 82f); data_QCG[191] = new Vector2(94f, 82f);
        data_MTCG[192] = new Vector2(76f, 82f); data_HWCG[192] = new Vector2(90f, 82f); data_QCG[192] = new Vector2(90f, 82f);
        data_MTCG[193] = new Vector2(90f, 82f); data_HWCG[193] = new Vector2(98f, 82f); data_QCG[193] = new Vector2(91f, 82f);
        data_MTCG[194] = new Vector2(85f, 82f); data_HWCG[194] = new Vector2(70f, 82f); data_QCG[194] = new Vector2(86f, 82f);
        data_MTCG[195] = new Vector2(98f, 82f); data_HWCG[195] = new Vector2(36f, 82f); data_QCG[195] = new Vector2(91f, 82f);
        data_MTCG[196] = new Vector2(86f, 82f); data_HWCG[196] = new Vector2(81f, 82f); data_QCG[196] = new Vector2(80f, 82f);
        data_MTCG[197] = new Vector2(84f, 82.8f); data_HWCG[197] = new Vector2(95.4f, 82.8f); data_QCG[197] = new Vector2(86.4f, 82.8f);
        data_MTCG[198] = new Vector2(75f, 83f); data_HWCG[198] = new Vector2(65f, 83f); data_QCG[198] = new Vector2(86f, 83f);
        data_MTCG[199] = new Vector2(89f, 83f); data_HWCG[199] = new Vector2(8f, 83f); data_QCG[199] = new Vector2(91f, 83f);
        data_MTCG[200] = new Vector2(93f, 83f); data_HWCG[200] = new Vector2(97f, 83f); data_QCG[200] = new Vector2(80f, 83f);
        data_MTCG[201] = new Vector2(80f, 83f); data_HWCG[201] = new Vector2(79f, 83f); data_QCG[201] = new Vector2(77f, 83f);
        data_MTCG[202] = new Vector2(77f, 83f); data_HWCG[202] = new Vector2(91f, 83f); data_QCG[202] = new Vector2(89f, 83f);
        data_MTCG[203] = new Vector2(85f, 83f); data_HWCG[203] = new Vector2(43f, 83f); data_QCG[203] = new Vector2(86f, 83f);
        data_MTCG[204] = new Vector2(90f, 83f); data_HWCG[204] = new Vector2(99f, 83f); data_QCG[204] = new Vector2(84f, 83f);
        data_MTCG[205] = new Vector2(75f, 83f); data_HWCG[205] = new Vector2(96f, 83f); data_QCG[205] = new Vector2(74f, 83f);
        data_MTCG[206] = new Vector2(89f, 83f); data_HWCG[206] = new Vector2(98f, 83f); data_QCG[206] = new Vector2(77f, 83f);
        data_MTCG[207] = new Vector2(73f, 83f); data_HWCG[207] = new Vector2(98f, 83f); data_QCG[207] = new Vector2(81f, 83f);
        data_MTCG[208] = new Vector2(67f, 83f); data_HWCG[208] = new Vector2(87f, 83f); data_QCG[208] = new Vector2(96f, 83f);
        data_MTCG[209] = new Vector2(82f, 83.2f); data_HWCG[209] = new Vector2(99.8f, 83.2f); data_QCG[209] = new Vector2(84.8f, 83.2f);
        data_MTCG[210] = new Vector2(86f, 83.5f); data_HWCG[210] = new Vector2(82f, 83.5f); data_QCG[210] = new Vector2(83.3f, 83.5f);
        data_MTCG[211] = new Vector2(91f, 83.6f); data_HWCG[211] = new Vector2(74.5f, 83.6f); data_QCG[211] = new Vector2(90.9f, 83.6f);
        data_MTCG[212] = new Vector2(85f, 84f); data_HWCG[212] = new Vector2(94f, 84f); data_QCG[212] = new Vector2(74f, 84f);
        data_MTCG[213] = new Vector2(82f, 84f); data_HWCG[213] = new Vector2(84f, 84f); data_QCG[213] = new Vector2(71f, 84f);
        data_MTCG[214] = new Vector2(82f, 84f); data_HWCG[214] = new Vector2(94f, 84f); data_QCG[214] = new Vector2(99f, 84f);
        data_MTCG[215] = new Vector2(84f, 84f); data_HWCG[215] = new Vector2(100f, 84f); data_QCG[215] = new Vector2(90f, 84f);
        data_MTCG[216] = new Vector2(85f, 84f); data_HWCG[216] = new Vector2(90f, 84f); data_QCG[216] = new Vector2(81f, 84f);
        data_MTCG[217] = new Vector2(76f, 84f); data_HWCG[217] = new Vector2(88f, 84f); data_QCG[217] = new Vector2(84f, 84f);
        data_MTCG[218] = new Vector2(90f, 84f); data_HWCG[218] = new Vector2(89f, 84f); data_QCG[218] = new Vector2(81f, 84f);
        data_MTCG[219] = new Vector2(82f, 84f); data_HWCG[219] = new Vector2(94f, 84f); data_QCG[219] = new Vector2(84f, 84f);
        data_MTCG[220] = new Vector2(74f, 84f); data_HWCG[220] = new Vector2(95f, 84f); data_QCG[220] = new Vector2(89f, 84f);
        data_MTCG[221] = new Vector2(88f, 84f); data_HWCG[221] = new Vector2(90f, 84f); data_QCG[221] = new Vector2(89f, 84f);
        data_MTCG[222] = new Vector2(72f, 84f); data_HWCG[222] = new Vector2(90f, 84f); data_QCG[222] = new Vector2(96f, 84f);
        data_MTCG[223] = new Vector2(79f, 84f); data_HWCG[223] = new Vector2(100f, 84f); data_QCG[223] = new Vector2(91f, 84f);
        data_MTCG[224] = new Vector2(87f, 84f); data_HWCG[224] = new Vector2(97f, 84f); data_QCG[224] = new Vector2(74f, 84f);
        data_MTCG[225] = new Vector2(74f, 84f); data_HWCG[225] = new Vector2(91f, 84f); data_QCG[225] = new Vector2(91f, 84f);
        data_MTCG[226] = new Vector2(90f, 84.1f); data_HWCG[226] = new Vector2(100f, 84.1f); data_QCG[226] = new Vector2(92.4f, 84.1f);
        data_MTCG[227] = new Vector2(92f, 84.9f); data_HWCG[227] = new Vector2(96.9f, 84.9f); data_QCG[227] = new Vector2(86.4f, 84.9f);
        data_MTCG[228] = new Vector2(90f, 85f); data_HWCG[228] = new Vector2(93f, 85f); data_QCG[228] = new Vector2(81f, 85f);
        data_MTCG[229] = new Vector2(85f, 85f); data_HWCG[229] = new Vector2(82f, 85f); data_QCG[229] = new Vector2(88f, 85f);
        data_MTCG[230] = new Vector2(83f, 85f); data_HWCG[230] = new Vector2(98f, 85f); data_QCG[230] = new Vector2(74f, 85f);
        data_MTCG[231] = new Vector2(78f, 85f); data_HWCG[231] = new Vector2(94f, 85f); data_QCG[231] = new Vector2(92f, 85f);
        data_MTCG[232] = new Vector2(92f, 85f); data_HWCG[232] = new Vector2(98f, 85f); data_QCG[232] = new Vector2(89f, 85f);
        data_MTCG[233] = new Vector2(82f, 85f); data_HWCG[233] = new Vector2(88f, 85f); data_QCG[233] = new Vector2(80f, 85f);
        data_MTCG[234] = new Vector2(90f, 85f); data_HWCG[234] = new Vector2(80f, 85f); data_QCG[234] = new Vector2(96f, 85f);
        data_MTCG[235] = new Vector2(87f, 85f); data_HWCG[235] = new Vector2(91f, 85f); data_QCG[235] = new Vector2(82f, 85f);
        data_MTCG[236] = new Vector2(78f, 85f); data_HWCG[236] = new Vector2(96f, 85f); data_QCG[236] = new Vector2(79f, 85f);
        data_MTCG[237] = new Vector2(68f, 85f); data_HWCG[237] = new Vector2(88f, 85f); data_QCG[237] = new Vector2(80f, 85f);
        data_MTCG[238] = new Vector2(93f, 85f); data_HWCG[238] = new Vector2(8f, 85f); data_QCG[238] = new Vector2(94f, 85f);
        data_MTCG[239] = new Vector2(79f, 85f); data_HWCG[239] = new Vector2(92f, 85f); data_QCG[239] = new Vector2(73f, 85f);
        data_MTCG[240] = new Vector2(93f, 85f); data_HWCG[240] = new Vector2(38f, 85f); data_QCG[240] = new Vector2(99f, 85f);
        data_MTCG[241] = new Vector2(81f, 85f); data_HWCG[241] = new Vector2(91f, 85f); data_QCG[241] = new Vector2(99f, 85f);
        data_MTCG[242] = new Vector2(100f, 85f); data_HWCG[242] = new Vector2(70f, 85f); data_QCG[242] = new Vector2(84f, 85f);
        data_MTCG[243] = new Vector2(89f, 85f); data_HWCG[243] = new Vector2(51f, 85f); data_QCG[243] = new Vector2(84f, 85f);
        data_MTCG[244] = new Vector2(86f, 85f); data_HWCG[244] = new Vector2(93f, 85f); data_QCG[244] = new Vector2(80f, 85f);
        data_MTCG[245] = new Vector2(91f, 85f); data_HWCG[245] = new Vector2(79f, 85f); data_QCG[245] = new Vector2(91f, 85f);
        data_MTCG[246] = new Vector2(84f, 85.7f); data_HWCG[246] = new Vector2(98.1f, 85.7f); data_QCG[246] = new Vector2(86.4f, 85.7f);
        data_MTCG[247] = new Vector2(87f, 85.8f); data_HWCG[247] = new Vector2(99.6f, 85.8f); data_QCG[247] = new Vector2(89.4f, 85.8f);
        data_MTCG[248] = new Vector2(85f, 85.9f); data_HWCG[248] = new Vector2(98.9f, 85.9f); data_QCG[248] = new Vector2(78.8f, 85.9f);
        data_MTCG[249] = new Vector2(82f, 86f); data_HWCG[249] = new Vector2(84f, 86f); data_QCG[249] = new Vector2(80f, 86f);
        data_MTCG[250] = new Vector2(82f, 86f); data_HWCG[250] = new Vector2(83f, 86f); data_QCG[250] = new Vector2(90f, 86f);
        data_MTCG[251] = new Vector2(83f, 86f); data_HWCG[251] = new Vector2(96f, 86f); data_QCG[251] = new Vector2(81f, 86f);
        data_MTCG[252] = new Vector2(88f, 86f); data_HWCG[252] = new Vector2(88f, 86f); data_QCG[252] = new Vector2(77f, 86f);
        data_MTCG[253] = new Vector2(74f, 86f); data_HWCG[253] = new Vector2(100f, 86f); data_QCG[253] = new Vector2(86f, 86f);
        data_MTCG[254] = new Vector2(94f, 86f); data_HWCG[254] = new Vector2(59f, 86f); data_QCG[254] = new Vector2(87f, 86f);
        data_MTCG[255] = new Vector2(94f, 86f); data_HWCG[255] = new Vector2(99f, 86f); data_QCG[255] = new Vector2(100f, 86f);
        data_MTCG[256] = new Vector2(88f, 86f); data_HWCG[256] = new Vector2(80f, 86f); data_QCG[256] = new Vector2(95f, 86f);
        data_MTCG[257] = new Vector2(84f, 86f); data_HWCG[257] = new Vector2(93f, 86f); data_QCG[257] = new Vector2(90f, 86f);
        data_MTCG[258] = new Vector2(74f, 86f); data_HWCG[258] = new Vector2(100f, 86f); data_QCG[258] = new Vector2(96f, 86f);
        data_MTCG[259] = new Vector2(79f, 86f); data_HWCG[259] = new Vector2(90f, 86f); data_QCG[259] = new Vector2(79f, 86f);
        data_MTCG[260] = new Vector2(90f, 86f); data_HWCG[260] = new Vector2(94f, 86f); data_QCG[260] = new Vector2(74f, 86f);
        data_MTCG[261] = new Vector2(74f, 86f); data_HWCG[261] = new Vector2(98f, 86f); data_QCG[261] = new Vector2(80f, 86f);
        data_MTCG[262] = new Vector2(89f, 86f); data_HWCG[262] = new Vector2(98f, 86f); data_QCG[262] = new Vector2(88f, 86f);
        data_MTCG[263] = new Vector2(93f, 86.8f); data_HWCG[263] = new Vector2(94.9f, 86.8f); data_QCG[263] = new Vector2(74.2f, 86.8f);
        data_MTCG[264] = new Vector2(92f, 87f); data_HWCG[264] = new Vector2(81f, 87f); data_QCG[264] = new Vector2(94f, 87f);
        data_MTCG[265] = new Vector2(91f, 87f); data_HWCG[265] = new Vector2(72f, 87f); data_QCG[265] = new Vector2(97f, 87f);
        data_MTCG[266] = new Vector2(74f, 87f); data_HWCG[266] = new Vector2(79f, 87f); data_QCG[266] = new Vector2(86f, 87f);
        data_MTCG[267] = new Vector2(95f, 87f); data_HWCG[267] = new Vector2(85f, 87f); data_QCG[267] = new Vector2(82f, 87f);
        data_MTCG[268] = new Vector2(83f, 87f); data_HWCG[268] = new Vector2(93f, 87f); data_QCG[268] = new Vector2(91f, 87f);
        data_MTCG[269] = new Vector2(94f, 87f); data_HWCG[269] = new Vector2(100f, 87f); data_QCG[269] = new Vector2(92f, 87f);
        data_MTCG[270] = new Vector2(89f, 87f); data_HWCG[270] = new Vector2(81f, 87f); data_QCG[270] = new Vector2(93f, 87f);
        data_MTCG[271] = new Vector2(80f, 87f); data_HWCG[271] = new Vector2(85f, 87f); data_QCG[271] = new Vector2(83f, 87f);
        data_MTCG[272] = new Vector2(91f, 87f); data_HWCG[272] = new Vector2(63f, 87f); data_QCG[272] = new Vector2(100f, 87f);
        data_MTCG[273] = new Vector2(83f, 87f); data_HWCG[273] = new Vector2(92f, 87f); data_QCG[273] = new Vector2(100f, 87f);
        data_MTCG[274] = new Vector2(88f, 87f); data_HWCG[274] = new Vector2(78f, 87f); data_QCG[274] = new Vector2(97f, 87f);
        data_MTCG[275] = new Vector2(91f, 87f); data_HWCG[275] = new Vector2(97f, 87f); data_QCG[275] = new Vector2(96f, 87f);
        data_MTCG[276] = new Vector2(84f, 87f); data_HWCG[276] = new Vector2(96f, 87f); data_QCG[276] = new Vector2(86f, 87f);
        data_MTCG[277] = new Vector2(74f, 87f); data_HWCG[277] = new Vector2(77f, 87f); data_QCG[277] = new Vector2(96f, 87f);
        data_MTCG[278] = new Vector2(83f, 87f); data_HWCG[278] = new Vector2(93f, 87f); data_QCG[278] = new Vector2(87f, 87f);
        data_MTCG[279] = new Vector2(87f, 87.4f); data_HWCG[279] = new Vector2(97.7f, 87.4f); data_QCG[279] = new Vector2(95.5f, 87.4f);
        data_MTCG[280] = new Vector2(86f, 87.9f); data_HWCG[280] = new Vector2(97.3f, 87.9f); data_QCG[280] = new Vector2(87.9f, 87.9f);
        data_MTCG[281] = new Vector2(88f, 88f); data_HWCG[281] = new Vector2(100f, 88f); data_QCG[281] = new Vector2(90f, 88f);
        data_MTCG[282] = new Vector2(88f, 88f); data_HWCG[282] = new Vector2(88f, 88f); data_QCG[282] = new Vector2(91f, 88f);
        data_MTCG[283] = new Vector2(79f, 88f); data_HWCG[283] = new Vector2(98f, 88f); data_QCG[283] = new Vector2(86f, 88f);
        data_MTCG[284] = new Vector2(87f, 88f); data_HWCG[284] = new Vector2(91f, 88f); data_QCG[284] = new Vector2(99f, 88f);
        data_MTCG[285] = new Vector2(73f, 88f); data_HWCG[285] = new Vector2(96f, 88f); data_QCG[285] = new Vector2(96f, 88f);
        data_MTCG[286] = new Vector2(91f, 88f); data_HWCG[286] = new Vector2(76f, 88f); data_QCG[286] = new Vector2(90f, 88f);
        data_MTCG[287] = new Vector2(84f, 88f); data_HWCG[287] = new Vector2(99f, 88f); data_QCG[287] = new Vector2(96f, 88f);
        data_MTCG[288] = new Vector2(86f, 88f); data_HWCG[288] = new Vector2(81f, 88f); data_QCG[288] = new Vector2(83f, 88f);
        data_MTCG[289] = new Vector2(87f, 88f); data_HWCG[289] = new Vector2(99f, 88f); data_QCG[289] = new Vector2(82f, 88f);
        data_MTCG[290] = new Vector2(94f, 88.5f); data_HWCG[290] = new Vector2(97.5f, 88.5f); data_QCG[290] = new Vector2(97f, 88.5f);
        data_MTCG[291] = new Vector2(82f, 89f); data_HWCG[291] = new Vector2(91f, 89f); data_QCG[291] = new Vector2(84f, 89f);
        data_MTCG[292] = new Vector2(91f, 89f); data_HWCG[292] = new Vector2(95f, 89f); data_QCG[292] = new Vector2(90f, 89f);
        data_MTCG[293] = new Vector2(90f, 89f); data_HWCG[293] = new Vector2(91f, 89f); data_QCG[293] = new Vector2(83f, 89f);
        data_MTCG[294] = new Vector2(85f, 89f); data_HWCG[294] = new Vector2(100f, 89f); data_QCG[294] = new Vector2(78f, 89f);
        data_MTCG[295] = new Vector2(95f, 89f); data_HWCG[295] = new Vector2(78f, 89f); data_QCG[295] = new Vector2(94f, 89f);
        data_MTCG[296] = new Vector2(81f, 89f); data_HWCG[296] = new Vector2(97f, 89f); data_QCG[296] = new Vector2(89f, 89f);
        data_MTCG[297] = new Vector2(87f, 89f); data_HWCG[297] = new Vector2(81f, 89f); data_QCG[297] = new Vector2(88f, 89f);
        data_MTCG[298] = new Vector2(83f, 89f); data_HWCG[298] = new Vector2(66f, 89f); data_QCG[298] = new Vector2(89f, 89f);
        data_MTCG[299] = new Vector2(96f, 89f); data_HWCG[299] = new Vector2(82f, 89f); data_QCG[299] = new Vector2(89f, 89f);
        data_MTCG[300] = new Vector2(88f, 89f); data_HWCG[300] = new Vector2(99.6f, 89f); data_QCG[300] = new Vector2(81.8f, 89f);
        data_MTCG[301] = new Vector2(92f, 89.8f); data_HWCG[301] = new Vector2(99.6f, 89.8f); data_QCG[301] = new Vector2(90.9f, 89.8f);
        data_MTCG[302] = new Vector2(88f, 90f); data_HWCG[302] = new Vector2(100f, 90f); data_QCG[302] = new Vector2(91f, 90f);
        data_MTCG[303] = new Vector2(84f, 90f); data_HWCG[303] = new Vector2(66f, 90f); data_QCG[303] = new Vector2(94f, 90f);
        data_MTCG[304] = new Vector2(90f, 90f); data_HWCG[304] = new Vector2(98f, 90f); data_QCG[304] = new Vector2(88f, 90f);
        data_MTCG[305] = new Vector2(89f, 90f); data_HWCG[305] = new Vector2(91f, 90f); data_QCG[305] = new Vector2(85f, 90f);
        data_MTCG[306] = new Vector2(95f, 90f); data_HWCG[306] = new Vector2(51f, 90f); data_QCG[306] = new Vector2(88f, 90f);
        data_MTCG[307] = new Vector2(94f, 90f); data_HWCG[307] = new Vector2(98f, 90f); data_QCG[307] = new Vector2(84f, 90f);
        data_MTCG[308] = new Vector2(94f, 90.3f); data_HWCG[308] = new Vector2(88.7f, 90.3f); data_QCG[308] = new Vector2(92.4f, 90.3f);
        data_MTCG[309] = new Vector2(95f, 90.7f); data_HWCG[309] = new Vector2(79.2f, 90.7f); data_QCG[309] = new Vector2(89.4f, 90.7f);
        data_MTCG[310] = new Vector2(86f, 91f); data_HWCG[310] = new Vector2(92f, 91f); data_QCG[310] = new Vector2(91f, 91f);
        data_MTCG[311] = new Vector2(84f, 91f); data_HWCG[311] = new Vector2(98f, 91f); data_QCG[311] = new Vector2(92f, 91f);
        data_MTCG[312] = new Vector2(90f, 91f); data_HWCG[312] = new Vector2(99f, 91f); data_QCG[312] = new Vector2(90f, 91f);
        data_MTCG[313] = new Vector2(93f, 91f); data_HWCG[313] = new Vector2(95f, 91f); data_QCG[313] = new Vector2(96f, 91f);
        data_MTCG[314] = new Vector2(89f, 91f); data_HWCG[314] = new Vector2(100f, 91f); data_QCG[314] = new Vector2(100f, 91f);
        data_MTCG[315] = new Vector2(86f, 91f); data_HWCG[315] = new Vector2(96f, 91f); data_QCG[315] = new Vector2(98f, 91f);
        data_MTCG[316] = new Vector2(90f, 91f); data_HWCG[316] = new Vector2(87f, 91f); data_QCG[316] = new Vector2(89f, 91f);
        data_MTCG[317] = new Vector2(95f, 91f); data_HWCG[317] = new Vector2(84f, 91f); data_QCG[317] = new Vector2(96f, 91f);
        data_MTCG[318] = new Vector2(91f, 91f); data_HWCG[318] = new Vector2(89f, 91f); data_QCG[318] = new Vector2(93f, 91f);
        data_MTCG[319] = new Vector2(90f, 91f); data_HWCG[319] = new Vector2(99f, 91f); data_QCG[319] = new Vector2(89f, 91f);
        data_MTCG[320] = new Vector2(92f, 91f); data_HWCG[320] = new Vector2(88f, 91f); data_QCG[320] = new Vector2(90f, 91f);
        data_MTCG[321] = new Vector2(97f, 91f); data_HWCG[321] = new Vector2(82.8f, 91f); data_QCG[321] = new Vector2(86.4f, 91f);
        data_MTCG[322] = new Vector2(74f, 91.1f); data_HWCG[322] = new Vector2(96.9f, 91.1f); data_QCG[322] = new Vector2(95.5f, 91.1f);
        data_MTCG[323] = new Vector2(93f, 92f); data_HWCG[323] = new Vector2(97f, 92f); data_QCG[323] = new Vector2(91f, 92f);
        data_MTCG[324] = new Vector2(87f, 92f); data_HWCG[324] = new Vector2(84f, 92f); data_QCG[324] = new Vector2(97f, 92f);
        data_MTCG[325] = new Vector2(91f, 92f); data_HWCG[325] = new Vector2(94f, 92f); data_QCG[325] = new Vector2(90f, 92f);
        data_MTCG[326] = new Vector2(89f, 92f); data_HWCG[326] = new Vector2(100f, 92f); data_QCG[326] = new Vector2(91f, 92f);
        data_MTCG[327] = new Vector2(86f, 92f); data_HWCG[327] = new Vector2(91f, 92f); data_QCG[327] = new Vector2(94f, 92f);
        data_MTCG[328] = new Vector2(88f, 92f); data_HWCG[328] = new Vector2(88f, 92f); data_QCG[328] = new Vector2(95f, 92f);
        data_MTCG[329] = new Vector2(95f, 92f); data_HWCG[329] = new Vector2(98f, 92f); data_QCG[329] = new Vector2(82f, 92f);
        data_MTCG[330] = new Vector2(95f, 92f); data_HWCG[330] = new Vector2(94f, 92f); data_QCG[330] = new Vector2(90f, 92f);
        data_MTCG[331] = new Vector2(82f, 92f); data_HWCG[331] = new Vector2(96f, 92f); data_QCG[331] = new Vector2(96f, 92f);
        data_MTCG[332] = new Vector2(95f, 92f); data_HWCG[332] = new Vector2(98f, 92f); data_QCG[332] = new Vector2(90f, 92f);
        data_MTCG[333] = new Vector2(95f, 92.4f); data_HWCG[333] = new Vector2(99.2f, 92.4f); data_QCG[333] = new Vector2(81.8f, 92.4f);
        data_MTCG[334] = new Vector2(100f, 93f); data_HWCG[334] = new Vector2(96f, 93f); data_QCG[334] = new Vector2(100f, 93f);
        data_MTCG[335] = new Vector2(87f, 93f); data_HWCG[335] = new Vector2(100f, 93f); data_QCG[335] = new Vector2(86f, 93f);
        data_MTCG[336] = new Vector2(91f, 93f); data_HWCG[336] = new Vector2(99f, 93f); data_QCG[336] = new Vector2(89f, 93f);
        data_MTCG[337] = new Vector2(96f, 93f); data_HWCG[337] = new Vector2(94f, 93f); data_QCG[337] = new Vector2(95f, 93f);
        data_MTCG[338] = new Vector2(96f, 93f); data_HWCG[338] = new Vector2(89f, 93f); data_QCG[338] = new Vector2(89f, 93f);
        data_MTCG[339] = new Vector2(88f, 93f); data_HWCG[339] = new Vector2(100f, 93f); data_QCG[339] = new Vector2(99f, 93f);
        data_MTCG[340] = new Vector2(94f, 93f); data_HWCG[340] = new Vector2(100f, 93f); data_QCG[340] = new Vector2(94f, 93f);
        data_MTCG[341] = new Vector2(90f, 93f); data_HWCG[341] = new Vector2(68f, 93f); data_QCG[341] = new Vector2(100f, 93f);
        data_MTCG[342] = new Vector2(98f, 93f); data_HWCG[342] = new Vector2(89f, 93f); data_QCG[342] = new Vector2(87f, 93f);
        data_MTCG[343] = new Vector2(94f, 93f); data_HWCG[343] = new Vector2(89f, 93f); data_QCG[343] = new Vector2(92f, 93f);
        data_MTCG[344] = new Vector2(93f, 93f); data_HWCG[344] = new Vector2(99f, 93f); data_QCG[344] = new Vector2(84f, 93f);
        data_MTCG[345] = new Vector2(89f, 93f); data_HWCG[345] = new Vector2(98f, 93f); data_QCG[345] = new Vector2(100f, 93f);
        data_MTCG[346] = new Vector2(94f, 93.8f); data_HWCG[346] = new Vector2(99.6f, 93.8f); data_QCG[346] = new Vector2(89.4f, 93.8f);
        data_MTCG[347] = new Vector2(86f, 94f); data_HWCG[347] = new Vector2(91f, 94f); data_QCG[347] = new Vector2(88f, 94f);
        data_MTCG[348] = new Vector2(91f, 94f); data_HWCG[348] = new Vector2(97f, 94f); data_QCG[348] = new Vector2(96f, 94f);
        data_MTCG[349] = new Vector2(90f, 94f); data_HWCG[349] = new Vector2(97f, 94f); data_QCG[349] = new Vector2(96f, 94f);
        data_MTCG[350] = new Vector2(99f, 94f); data_HWCG[350] = new Vector2(87f, 94f); data_QCG[350] = new Vector2(92f, 94f);
        data_MTCG[351] = new Vector2(96f, 94f); data_HWCG[351] = new Vector2(100f, 94f); data_QCG[351] = new Vector2(100f, 94f);
        data_MTCG[352] = new Vector2(93f, 94f); data_HWCG[352] = new Vector2(100f, 94f); data_QCG[352] = new Vector2(100f, 94f);
        data_MTCG[353] = new Vector2(95f, 95f); data_HWCG[353] = new Vector2(100f, 95f); data_QCG[353] = new Vector2(89f, 95f);
        data_MTCG[354] = new Vector2(97f, 95f); data_HWCG[354] = new Vector2(100f, 95f); data_QCG[354] = new Vector2(100f, 95f);
        data_MTCG[355] = new Vector2(88f, 95f); data_HWCG[355] = new Vector2(98f, 95f); data_QCG[355] = new Vector2(96f, 95f);
        data_MTCG[356] = new Vector2(97f, 95f); data_HWCG[356] = new Vector2(100f, 95f); data_QCG[356] = new Vector2(97f, 95f);
        data_MTCG[357] = new Vector2(96f, 95f); data_HWCG[357] = new Vector2(100f, 95f); data_QCG[357] = new Vector2(100f, 95f);
        data_MTCG[358] = new Vector2(95f, 95f); data_HWCG[358] = new Vector2(100f, 95f); data_QCG[358] = new Vector2(91f, 95f);
        data_MTCG[359] = new Vector2(91f, 95.5f); data_HWCG[359] = new Vector2(100f, 95.5f); data_QCG[359] = new Vector2(87.9f, 95.5f);
        data_MTCG[360] = new Vector2(97f, 96f); data_HWCG[360] = new Vector2(96f, 96f); data_QCG[360] = new Vector2(100f, 96f);
        data_MTCG[361] = new Vector2(94f, 96f); data_HWCG[361] = new Vector2(88f, 96f); data_QCG[361] = new Vector2(95f, 96f);
        data_MTCG[362] = new Vector2(100f, 96f); data_HWCG[362] = new Vector2(81f, 96f); data_QCG[362] = new Vector2(94f, 96f);
        data_MTCG[363] = new Vector2(99f, 96f); data_HWCG[363] = new Vector2(100f, 96f); data_QCG[363] = new Vector2(94f, 96f);
        data_MTCG[364] = new Vector2(99f, 96f); data_HWCG[364] = new Vector2(95f, 96f); data_QCG[364] = new Vector2(98f, 96f);
        data_MTCG[365] = new Vector2(99f, 96f); data_HWCG[365] = new Vector2(98f, 96f); data_QCG[365] = new Vector2(100f, 96f);
        data_MTCG[366] = new Vector2(93f, 96f); data_HWCG[366] = new Vector2(99f, 96f); data_QCG[366] = new Vector2(93f, 96f);
        data_MTCG[367] = new Vector2(99f, 96f); data_HWCG[367] = new Vector2(71f, 96f); data_QCG[367] = new Vector2(100f, 96f);
        data_MTCG[368] = new Vector2(89f, 96f); data_HWCG[368] = new Vector2(83f, 96f); data_QCG[368] = new Vector2(92f, 96f);
        data_MTCG[369] = new Vector2(97f, 96.1f); data_HWCG[369] = new Vector2(99.6f, 96.1f); data_QCG[369] = new Vector2(97f, 96.1f);
        data_MTCG[370] = new Vector2(97f, 96.2f); data_HWCG[370] = new Vector2(100f, 96.2f); data_QCG[370] = new Vector2(90.9f, 96.2f);
        data_MTCG[371] = new Vector2(91f, 97f); data_HWCG[371] = new Vector2(100f, 97f); data_QCG[371] = new Vector2(100f, 97f);
        data_MTCG[372] = new Vector2(90f, 97f); data_HWCG[372] = new Vector2(95f, 97f); data_QCG[372] = new Vector2(95f, 97f);
        data_MTCG[373] = new Vector2(89f, 97f); data_HWCG[373] = new Vector2(99f, 97f); data_QCG[373] = new Vector2(100f, 97f);
        data_MTCG[374] = new Vector2(97f, 97f); data_HWCG[374] = new Vector2(100f, 97f); data_QCG[374] = new Vector2(100f, 97f);
        data_MTCG[375] = new Vector2(99f, 97.7f); data_HWCG[375] = new Vector2(91.8f, 97.7f); data_QCG[375] = new Vector2(97f, 97.7f);
        data_MTCG[376] = new Vector2(98f, 98f); data_HWCG[376] = new Vector2(96f, 98f); data_QCG[376] = new Vector2(100f, 98f);
        data_MTCG[377] = new Vector2(99f, 98.4f); data_HWCG[377] = new Vector2(92.4f, 98.4f); data_QCG[377] = new Vector2(97f, 98.4f);
        data_MTCG[378] = new Vector2(96f, 98.9f); data_HWCG[378] = new Vector2(100f, 98.9f); data_QCG[378] = new Vector2(100f, 98.9f);
        data_MTCG[379] = new Vector2(98f, 99f); data_HWCG[379] = new Vector2(99f, 99f); data_QCG[379] = new Vector2(100f, 99f);
        data_MTCG[380] = new Vector2(100f, 99f); data_HWCG[380] = new Vector2(99f, 99f); data_QCG[380] = new Vector2(93f, 99f);
        data_MTCG[381] = new Vector2(95f, 99f); data_HWCG[381] = new Vector2(100f, 99f); data_QCG[381] = new Vector2(100f, 99f);
        data_MTCG[382] = new Vector2(98f, 99f); data_HWCG[382] = new Vector2(97f, 99f); data_QCG[382] = new Vector2(100f, 99f);
        data_MTCG[383] = new Vector2(96f, 100f); data_HWCG[383] = new Vector2(94f, 100f); data_QCG[383] = new Vector2(100f, 100f);
        data_MTCG[384] = new Vector2(99f, 100f); data_HWCG[384] = new Vector2(99.2f, 100f); data_QCG[384] = new Vector2(98.5f, 100f);




    }
}
