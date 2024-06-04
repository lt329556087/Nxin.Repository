using System;
using System.Text;

namespace FinanceManagement.Util
{
    public class TheRMBCapital
    {
        /// <summary>
        /// 人民币大写转换
        /// </summary>
        /// <param name="strNum"></param>
        /// <returns></returns>
        public static string Transform(string strNum)
        {
            var resultStr = new StringBuilder();
            bool f = true;
            decimal d = 0;
            string s = "";
            try
            {
                d = decimal.Parse(strNum);

            }
            catch (Exception e) { f = false; resultStr.Append("数字字符串不合法"); }
            if (f)
            {
                string[] numStr = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
                string[] intUnit = { "", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿" };
                string[] decUnit = { "角", "分" };

                if (d < 0)
                {
                    // resultStr.Append("负");
                    s = "负";
                    strNum = Math.Abs(d).ToString();
                }
                else if (d == 0)
                {
                    resultStr.Append("零元整");
                    return resultStr.ToString();
                }

                //小数点的位置
                int pIndex = strNum.IndexOf(".");
                pIndex = pIndex >= 0 ? pIndex : strNum.Length;
                //整数部分
                string intStr = strNum.Substring(0, pIndex);
                //小数部分
                string decStr = pIndex >= strNum.Length ? "" : strNum.Substring(pIndex + 1, strNum.Length - 1 - pIndex);
                //开始转换整数
                getIntStr(intStr, ref numStr, ref intUnit, resultStr);
                if (resultStr.Length > 0) resultStr.Append("元");
                //开始转换小数
                int p = int.Parse(decStr);
                if (p == 0) decStr = "";
                getDecStr(decStr, ref numStr, ref decUnit, resultStr);
            }
            resultStr.Insert(0, s);
            return resultStr.ToString();
        }

        private static void getIntStr(string intStr, ref String[] numStr, ref String[] intUnit, StringBuilder resultStr)
        {
            string hBitStr = intStr.Length > intUnit.Length - 1 ? intStr.Substring(0, intStr.Length - intUnit.Length + 1) : "";
            string lUnitStr = intStr.Substring(hBitStr.Length, intStr.Length - hBitStr.Length);
            var unitStr = new StringBuilder();
            bool hasZero = false;
            int bitIndex = 0;
            while (bitIndex < lUnitStr.Length)
            {
                int n = int.Parse(lUnitStr.Substring(bitIndex, 1));
                int IU = lUnitStr.Length - bitIndex - 1;
                if (n != 0)
                {
                    string NU = numStr[n] + intUnit[IU];
                    if (hasZero)
                    {
                        unitStr.Append(numStr[0] + NU);
                        hasZero = false;
                    }
                    else
                        unitStr.Append(NU);
                }
                else if (IU == 4 || IU == 8) //如果为0 切该位是“万”或“亿”则加上其位名
                { unitStr.Append(intUnit[IU]); }
                else hasZero = true;
                bitIndex++;
            }
            if (resultStr.Length > 0) unitStr.Append(intUnit[intUnit.Length - 1]);
            resultStr.Insert(0, unitStr);
            if (hBitStr.Length > 0)
                getIntStr(hBitStr, ref numStr, ref intUnit, resultStr);
        }
        private static void getDecStr(string decStr, ref String[] numStr, ref String[] decUnit, StringBuilder resultStr)
        {
            var unitStr = new StringBuilder();
            bool hasZero = false;
            int bitIndex = 0;
            if (decStr.Length > 0)
            {
                while (bitIndex < decUnit.Length && bitIndex < decStr.Length)
                {
                    int n = int.Parse(decStr.Substring(bitIndex, 1));
                    if (n != 0)
                    {
                        string NU = numStr[n] + decUnit[bitIndex];
                        if (hasZero)
                        {
                            unitStr.Append(numStr[0] + NU);
                            hasZero = false;
                        }
                        else
                            unitStr.Append(NU);
                    }
                    else
                    {
                        hasZero = true;
                        if (bitIndex == decStr.Length - 1)
                            unitStr.Append("整");
                    }
                    bitIndex++;
                }

                resultStr.Append(unitStr);
            }
            else resultStr.Append("整");
        }
    }
}
