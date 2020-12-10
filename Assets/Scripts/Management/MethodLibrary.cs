using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class MethodLibrary : MonoBehaviour
{ 
    /// <summary>
    /// 0 < "Modulus" 
    /// </summary>
    public int Modulus;

    /// <summary>
    /// 0 < "Multiplier" < "Modulus"
    /// </summary>
    public int Multiplier;

    /// <summary>
    /// 0 <= "Increment" < "Modulus"
    /// </summary>
    public int Increment;

    /// <summary>
    /// "lastResult" holds the previous result of the randomizer
    /// </summary>
    public int lastResult;
    public enum PuzzleType { portalPuzzle, ghostPuzzle, fakePuzzle, tiltPuzzle, wispPuzzle, backwardsPuzzle, tagPuzzle };
    #region Conversions
    /// <summary>
    /// Returns the given hex string as an int dec number
    /// </summary>
    /// <param name="hexString"></param>
    /// <returns></returns>
    public int HexToDec(string hexString)
    {
        return int.Parse(hexString, NumberStyles.HexNumber);
    }
    /// <summary>
    /// Returns the given hex string as an float dec number
    /// </summary>
    /// <param name="hexString"></param>
    /// <returns></returns>
    public float HexToDecF(string hexString)
    {
        //split string and put the frac on the right side of decimal. reverse  of turing to string
        string[] hold = hexString.Split('.');
        int left = int.Parse(hold[0], NumberStyles.HexNumber);
        print(left);
        float right = int.Parse(hold[1], NumberStyles.HexNumber);
        print(right);
        right = right * Mathf.Pow(10, -(hold[1].Length + 1));
        return left + right;
    }
    /// <summary>
    /// Returns the given dec number as a hex string
    /// </summary>
    /// <param name="decNumber"></param>
    /// <returns></returns>
    public string DecToHex(int decNumber)
    {
        return decNumber.ToString("x");
    }
    #endregion
    #region Generations
    #region SeedSpecific
    /// <summary>
    /// Returns a seed to used
    /// </summary>
    /// <param name="maxDifficulty"> Maximum difficulty </param>
    /// <param name="chances"> int array to store puzzle chances </param>
    /// <returns></returns>
    public string GenerateSeed(int lvlNum , int maxDifficulty, int[] chances)
    {
        float diff = ((((lvlNum + 1)/maxDifficulty)*10)+1);
        ///
        int puzzleType = SkewedNum(0, chances[0], 1, chances[1], 2, chances[2], 3, chances[3], 4, chances[4], 5, chances[5]);
        int mushSpot = SkewedNum(0, 1, 1, 1, 2, 1, 3, 1, 4, 1);
        int terrainMap = SkewedNum(0, 1, 1, 1, 2, 1);
        int lvlSP = SkewedNum(0, 1, 1, 1, 2, 1, 3, 1, 4, 1, 5, 1);
        ///
        return DecToHex(lvlNum+1) + "r" + 
            //puzzleType
            DecToHex(puzzleType) + "r" + 
            //ShroomSpot
            DecToHex(mushSpot) + "r" + 
            //TerrainMap
            DecToHex(terrainMap) + "r" + 
            //lvlSpecific
            DecToHex(lvlSP)  + "r" + 
            //Difficulty
            DecToHex((int)diff) + "r" +
            //puzzleChances
            DecToHex(chances[0]) + "r" + DecToHex(chances[1]) + "r" + 
            DecToHex(chances[2]) + "r" + DecToHex(chances[3]) + "r" + 
            DecToHex(chances[4]) + "r" + DecToHex(chances[5]);

        /// 11 fields

    }
    #endregion
    #endregion
    #region MiscMethods   
    /// <summary>
    /// returns true if a is below b
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool IsBelow(Vector3 a, Vector3 b)
    {
        return a.y < b.y;
    }

    /// <summary>
    /// Alters the puzzle type probabilities
    /// </summary>
    /// <param name="puzzleType"> Enter the puzzle type of current </param>
    public void AlterProbabilities(PuzzleType puzzleType, int probability1, int probability2, 
        int probability3, int probability4, int probability5)
    {
        probability3 = 5;
        if(puzzleType == PuzzleType.portalPuzzle)
        {
            probability1 -= 10;
            probability2 += 10;
            probability3 += 5;
            probability4 += 10;
            probability5 += 10;
        } 
        else if (puzzleType == PuzzleType.ghostPuzzle)
        {
            probability1 += 5;
            probability2 -= 10;
            probability3 += 5;
            probability4 += 10;
            probability5 += 10;
        }
        else if (puzzleType == PuzzleType.fakePuzzle)
        {
            probability1 += 5;
            probability2 += 10;
            probability3 -= 10;
            probability4 += 10;
            probability5 += 10;
        }
        else if (puzzleType == PuzzleType.tiltPuzzle)
        {
            probability1 += 5;
            probability2 += 10;
            probability3 += 5;
            probability4 -= 10;
            probability5 += 10;
        }
        else if (puzzleType == PuzzleType.wispPuzzle)
        {
            probability1 += 5;
            probability2 += 10;
            probability3 += 5;
            probability4 += 10;
            probability5 -= 10;
        }
        else if (puzzleType == PuzzleType.backwardsPuzzle)
        {
            probability1 += 5;
            probability2 += 10;
            probability3 += 5;
            probability4 += 10;
            probability5 -= 10;
        }
        else if (puzzleType == PuzzleType.tagPuzzle)
        {
            probability1 += 5;
            probability2 += 10;
            probability3 += 5;
            probability4 += 10;
            probability5 += 10;
        }
        if (probability1 < 0)
            probability1 = 0;
        if (probability2 < 0)
            probability2 = 0;
        if (probability3 < 0)
            probability3 = 0;
        if (probability4 < 0)
            probability4 = 0;
        if (probability5 < 0)
            probability5 = 0;
    }
    /// <summary> Returns an int of a skewed selection, every value needs a following p to appear </summary>
    /// <param name="v1"> Possible v to return </param>
    /// <param name="p1"> Chance to return corresponding value </param>
    /// <returns></returns>
    public int SkewedNum(int v1, int p1, int v2, int p2, int v3 = -1, int p3 = -1,
        int v4 = -1, int p4 = -1, int v5 = -1, int p5 = -1, int v6 = -1, int p6 = -1,
        int v7 = -1, int p7 = -1, int v8 = -1, int p8 = -1, int v9 = -1, int p9 = -1,
        int v10 = -1, int p10 = -1, int v11 = -1, int p11 = -1, int v12 = -1, int p12 = -1,
        int v13 = -1, int p13 = -1, int v14 = -1, int p14 = -1, int v15 = -1, int p15 = -1)
    {
        float totalP = 0, miscHold = 0;
        float[] oneToHundred = new float[16]; int[] valStorage = new int[15];

        valStorage[0] = v1; valStorage[1] = v2; valStorage[2] = v3; valStorage[3] = v4; 
        valStorage[4] = v5; valStorage[5] = v6; valStorage[6] = v7; valStorage[7] = v8;  
        valStorage[8] = v9; valStorage[9] = v10; valStorage[10] = v11; valStorage[11] = v12;
        valStorage[12] = v13; valStorage[13] = v14; valStorage[14] = v15;

        float[] augmentedP = new float[15];
        augmentedP[0] = p1; augmentedP[1] = p2; augmentedP[2] = p3; augmentedP[3] = p4;
        augmentedP[4] = p5; augmentedP[5] = p6; augmentedP[6] = p7; augmentedP[7] = p8;
        augmentedP[8] = p9; augmentedP[9] = p10; augmentedP[10] = p11; augmentedP[11] = p12;
        augmentedP[12] = p13; augmentedP[13] = p14; augmentedP[14] = p15;

        for (int counter = 0; counter < augmentedP.Length; counter++)
            if (augmentedP[counter] != -1)
                totalP += augmentedP[counter];

        for (int counter = 0; counter < augmentedP.Length; counter++)
            if (augmentedP[counter] != -1)
            {
                miscHold += augmentedP[counter];
                oneToHundred[counter + 1] = (miscHold / totalP) - 0.01f;
            }

        float hold = Random.Range(0f,1f);
        for (int counter = 1; counter < oneToHundred.Length; counter++)
            if (oneToHundred[counter - 1] <= hold && hold < oneToHundred[counter])
                return valStorage[counter - 1];

        return v1;
    }
    /// <summary>
    /// Returns an int[] after parsing "seed"
    /// </summary>
    /// <param name="seed"></param>
    /// <returns></returns>
    public int[] ParseSeed(string seed)
    {
        string[] stringArray = seed.Split('r');
        int[] placeHolder = new int[stringArray.Length];
        for(int counter = 0; counter < stringArray.Length; counter++)
            placeHolder[counter] = HexToDec(stringArray[counter]);

        return placeHolder;
    }
    /// <summary>
    /// Return a float[] after parsing "vector"
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public float[] ParseVector3(string vector)
    {
        string[] stringArray = vector.Split('\t');
        float[] placeHolder = new float[stringArray.Length];
        for (int counter = 0; counter < stringArray.Length; counter++)
            placeHolder[counter] = float.Parse(stringArray[counter]);

        return placeHolder;
    }

    public string toSeed(int[] vals)
    {
        string toReturn = "";
        for(int count = 0; count < vals.Length; count++)
        {
            toReturn += DecToHex(vals[count]) + (count + 1 < vals.Length ? "r" : "");
        }
        return toReturn;
    }
    public string toSeed(List<int> vals)
    {
        string toReturn = "";
        for (int count = 0; count < vals.Count; count++)
        {
            toReturn += DecToHex(vals[count]) + (count + 1 < vals.Count ? "r" : "");
        }
        return toReturn;
    }

    void ScrambleLCG(int max)
    {
        //int modMin = 4;
        //while (max <= 1)
        //    max++;
        //while (modMin >= max)
        //    modMin--;
        Modulus = max;
        Multiplier = Random.Range(1, Modulus);
        //print(Multiplier + "mul");
        Increment = Random.Range(1, Multiplier);
        //print(Increment + "inc");
        if (Increment == Multiplier)
            Multiplier++;
    }


    /// <summary>
    /// Generates a random number using the already existing random class and the last random number
    /// </summary>
    /// <param name="min">
    /// "min" is the lowest value for "Increment" [inclusive]
    /// </param>
    /// <param name="modMin">
    /// "modMin" is the lowest value for "Modulus" [inclusive]
    /// </param>
    /// <param name="max">
    /// "max" is the highest possible value for "Modulus" [exclusive]
    /// </param>
    /// <param name="rescramble"></param>
    /// <returns></returns>
    public int RandomNum(int max = 10, bool rescramble = true, int mult = 10, int inc = 4)
    {
        if (rescramble)
            ScrambleLCG(max);
        else
        {
            Modulus = max;
            Multiplier = mult;
            Increment = inc;
        }
        lastResult = ((Multiplier * lastResult) + Increment) % Modulus;
        return lastResult;
    }

    #endregion
}