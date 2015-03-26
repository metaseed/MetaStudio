using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using Microsoft.Practices.ServiceLocation;

using Catel.Data;
using Catel.Logging;
namespace Metaseed.Alogrithm
{
    public class BooleanExpressionToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObservableCollection<IBooleanUnit> OutputCondition = (ObservableCollection<IBooleanUnit>)value;
            if (OutputCondition == null || OutputCondition.Count == 0)
            {
                return "尚未配置(注：化简按钮只能得到最小项表达式，要得到最简表达式请对最小项表达式进行化简，比如用卡诺图法)";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in OutputCondition)
                {
                    sb.Append(item.Name);
                    sb.Append(" ");
                }
                return sb.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public interface IBooleanUnit
    {
        string Name { get; set; }
    }
    public class BooleanVeriable : ObservableObject, IBooleanUnit
    {
        string _Name;
        public string Name { get { return _Name; } set { _Name = value; RaisePropertyChanged("Name"); } }
        public static BooleanVeriable AlwaysOpen = new BooleanVeriable() { Name = "True", Index = 1 };
        public static BooleanVeriable AlwaysClose = new BooleanVeriable() { Name = "False", Index = 0 };
        public virtual UInt16 Index { get; set; }
    }
    public class LogicalOperator : IBooleanUnit
    {
        public string Name { get; set; }
        public static LogicalOperator Or = new LogicalOperator() { Name = "||" };
        public static LogicalOperator And = new LogicalOperator() { Name = "&&" };
        public static LogicalOperator Neg = new LogicalOperator() { Name = "!" };

    }
    public class Parenthesis : IBooleanUnit
    {
        public string Name { get; set; }
        public static Parenthesis LeftParenthesis = new Parenthesis() { Name = "(" };
        public static Parenthesis RightParenthesis = new Parenthesis() { Name = ")" };
    }
    public static class BooleanAlgebra
    {
        static ILog Logger = LogManager.GetCurrentClassLogger();
        static void Log(string message)
        {
            if (Logger != null)
            {
                Logger.Debug(message);
            }
        }
        static void NegParenthesis(ObservableCollection<IBooleanUnit> condition, ref int leftParenthesisIndex, ref int rightParenthesisIndex)
        {
            System.Diagnostics.Debug.Assert(condition[leftParenthesisIndex + 1] is BooleanVeriable || condition[leftParenthesisIndex + 1] == LogicalOperator.Neg);
            bool hasOr = false;
            int lastUnitFirstIndex = leftParenthesisIndex + 1;
            for (int orIndex = leftParenthesisIndex + 1; orIndex < rightParenthesisIndex; orIndex++)
            {
                if (condition[orIndex] == LogicalOperator.Or)
                {
                    LogicalOperator or = (LogicalOperator)condition[orIndex];
                    hasOr = true;
                    condition.Insert(lastUnitFirstIndex, Parenthesis.LeftParenthesis); rightParenthesisIndex++; orIndex++;
                    condition.Insert(lastUnitFirstIndex, LogicalOperator.Neg); rightParenthesisIndex++; orIndex++;
                    condition.Insert(orIndex, Parenthesis.RightParenthesis); rightParenthesisIndex++; orIndex++;
                    condition[orIndex] = LogicalOperator.And;
                    lastUnitFirstIndex = orIndex + 1;
                }
            }
            if (hasOr)
            {
                condition.Insert(lastUnitFirstIndex, Parenthesis.LeftParenthesis); rightParenthesisIndex++;
                condition.Insert(lastUnitFirstIndex, LogicalOperator.Neg); rightParenthesisIndex++;
                condition.Insert(rightParenthesisIndex, Parenthesis.RightParenthesis); rightParenthesisIndex++;
            }
            else
            {
                for (int i = leftParenthesisIndex + 1; i < rightParenthesisIndex; i++)
                {
                    System.Diagnostics.Debug.Assert(condition[i] is BooleanVeriable || condition[i] == LogicalOperator.Neg || condition[i] == LogicalOperator.And);
                    System.Diagnostics.Debug.Assert(condition[rightParenthesisIndex - 1] is BooleanVeriable);
                    if (condition[i] == LogicalOperator.And)
                    {
                        condition[i] = LogicalOperator.Or;
                    }
                    else
                    {
                        if (condition[i] == LogicalOperator.Neg)
                        {
                            condition.RemoveAt(i);
                            rightParenthesisIndex--;
                            System.Diagnostics.Debug.Assert(condition[i] is BooleanVeriable);
                        }
                        else
                        {
                            System.Diagnostics.Debug.Assert(condition[i] is BooleanVeriable);
                            condition.Insert(i, LogicalOperator.Neg); rightParenthesisIndex++; i++;
                        }
                    }
                }
            }
            condition.RemoveAt(leftParenthesisIndex - 1); leftParenthesisIndex--; rightParenthesisIndex--;//remove !
        }
        static void AndParethesis_Left(ObservableCollection<IBooleanUnit> condition, ref int leftParethesisIndex, ref int rightParethesisIndex)
        {
            System.Diagnostics.Debug.Assert(condition[leftParethesisIndex + 1] is BooleanVeriable || condition[leftParethesisIndex + 1] == LogicalOperator.Neg);
            System.Diagnostics.Debug.Assert(condition[leftParethesisIndex - 1] == LogicalOperator.And);
            System.Diagnostics.Debug.Assert(condition[leftParethesisIndex - 2] is BooleanVeriable);
            int indexOfAnd = leftParethesisIndex - 1;
            int leftEnd = indexOfAnd - 1;
            int leftStart = leftEnd;
            for (int k = leftStart; k >= 0 && condition[k] != Parenthesis.LeftParenthesis && condition[k] != LogicalOperator.Or; k--)
            {
                leftStart = k;
            }
            //List<LogicalOperator> orList = new List<LogicalOperator>();


            List<IBooleanUnit> lefts = new List<IBooleanUnit>();
            for (int lf = leftStart; lf <= leftEnd; lf++)
            {
                lefts.Add(condition[lf]);
            }
            for (int lf = leftStart; lf <= leftEnd; lf++)
            {
                condition.RemoveAt(leftStart); leftParethesisIndex--; rightParethesisIndex--;
            }
            condition.RemoveAt(leftStart); leftParethesisIndex--; rightParethesisIndex--;//remove &&

            condition.Insert(leftParethesisIndex + 1, LogicalOperator.And); rightParethesisIndex++;
            for (int bb = lefts.Count - 1; bb >= 0; bb--)
            {
                condition.Insert(leftParethesisIndex + 1, lefts[bb]); rightParethesisIndex++;
            }

            for (int i = leftParethesisIndex + 1 + lefts.Count + 1/*&&*/; i < rightParethesisIndex; i++)
            {
                if (condition[i] == LogicalOperator.Or)
                {
                    condition.Insert(i + 1, LogicalOperator.And); rightParethesisIndex++;
                    for (int bb = lefts.Count - 1; bb >= 0; bb--)
                    {
                        condition.Insert(i + 1, lefts[bb]); rightParethesisIndex++;
                    }
                }
            }
        }
        static bool AndParethesis_Right(ObservableCollection<IBooleanUnit> condition, ref  int leftParethesisIndex, ref int rightParethesisIndex)
        {
            System.Diagnostics.Debug.Assert(condition[rightParethesisIndex - 1] is BooleanVeriable ||
                condition[rightParethesisIndex - 1] == Parenthesis.RightParenthesis);
            System.Diagnostics.Debug.Assert(condition[rightParethesisIndex + 1] == LogicalOperator.And);
            System.Diagnostics.Debug.Assert(
                condition[rightParethesisIndex + 2] is BooleanVeriable ||
                condition[rightParethesisIndex + 2] == Parenthesis.LeftParenthesis ||
                condition[rightParethesisIndex + 2] == LogicalOperator.Neg
                );
            int indexOfAnd = rightParethesisIndex + 1;

            int rightStart = indexOfAnd + 1;
            int rightEnd = rightStart;
            if (condition[rightStart] == Parenthesis.LeftParenthesis ||
                (condition[rightStart] == LogicalOperator.Neg && condition[rightStart + 1] == Parenthesis.LeftParenthesis)
                )
            {
                for (int k = rightStart; k < condition.Count; k++)
                {
                    if (condition[k] == Parenthesis.RightParenthesis)
                    {
                        rightEnd = k;
                        break;
                    }
                    if (k == condition.Count)
                    {
                        MessageBox.Show("括号合并时:(A+B)(C+D，发现右括号不匹配，请输入右括号");
                        return false;
                    }
                }
            }
            else
            {
                int k;
                for (k = rightStart; k < condition.Count && condition[k] != LogicalOperator.Or && condition[k] != Parenthesis.RightParenthesis; k++)
                {
                    rightEnd = k;
                }
            }
            List<IBooleanUnit> rights = new List<IBooleanUnit>();

            for (int lf = rightStart; lf <= rightEnd; lf++)
            {
                rights.Add(condition[lf]);
            }
            for (int lf = rightStart; lf <= rightEnd; lf++)
            {
                condition.RemoveAt(rightStart);
            }
            condition.RemoveAt(rightStart - 1);//remove &&

            for (int bb = rights.Count - 1; bb >= 0; bb--)
            {
                condition.Insert(rightParethesisIndex, rights[bb]);
            }
            condition.Insert(rightParethesisIndex, LogicalOperator.And);
            int oldRightParethesisIndex = rightParethesisIndex;
            rightParethesisIndex += (rights.Count + 1);
            for (int i = leftParethesisIndex + 1; i < oldRightParethesisIndex; i++)
            {
                if (condition[i] == LogicalOperator.Or)
                {
                    condition.Insert(i, LogicalOperator.And); rightParethesisIndex++; i++; oldRightParethesisIndex++;
                    for (int bb = 0; bb < rights.Count; bb++)
                    {
                        condition.Insert(i, rights[bb]); rightParethesisIndex++; i++; oldRightParethesisIndex++;
                    }

                }
            }
            return true;
        }
        //http://www.allaboutcircuits.com/vol_4/chpt_7/5.html
        //http://webster.cs.ucr.edu/AoA/Windows/HTML/DigitalDesigna3.html
        //http://en.wikipedia.org/wiki/Boolean_algebra
        static bool simplify(ObservableCollection<IBooleanUnit> condition)
        {

            if (condition == null || condition.Count == 0)
            {
                return true;
            }
            BooleanExpressionToStringConverter cv = new BooleanExpressionToStringConverter();
            System.Console.WriteLine("表达式：");
            Log("Expression:");
            System.Console.WriteLine(cv.Convert(condition, null, null, null));
            Log(cv.Convert(condition, null, null, null).ToString());
            //remove meaningless parenthesis
            //(ABC)(!A)=ABC!A
            int leftParenthesisIndex_toRemove = -1;
            int rightParennthesisIndex_toRemove = -1;
            bool toRemoveAgain = true;
            while (toRemoveAgain)
            {
                toRemoveAgain = false;
                for (int i = 0; i < condition.Count; i++)
                {
                    if (condition[i] == Parenthesis.LeftParenthesis)
                    {
                        leftParenthesisIndex_toRemove = i;
                    }
                    else if (condition[i] == LogicalOperator.Or)
                    {
                        leftParenthesisIndex_toRemove = -1;
                    }
                    else if (condition[i] == Parenthesis.RightParenthesis && leftParenthesisIndex_toRemove != -1)
                    {
                        //remove
                        rightParennthesisIndex_toRemove = i;
                        condition.RemoveAt(leftParenthesisIndex_toRemove);
                        condition.RemoveAt(rightParennthesisIndex_toRemove - 1);
                        System.Console.WriteLine("去除无意义的括号：");
                        Log("Del meaningless parenthesises");
                        System.Console.WriteLine(cv.Convert(condition, null, null, null));
                        Log(cv.Convert(condition, null, null, null).ToString());
                        toRemoveAgain = true;
                        break;
                    }
                }
            }

            //end remove meanlingless parenthesis

            //find a pair of inner parenthesis
            int lastLeftParenthesisIndex = -1;
            int lastRightParenthesisIndex = -1;
            for (int i = 0; i < condition.Count; i++)
            {
                if (condition[i] == Parenthesis.LeftParenthesis)
                {
                    lastLeftParenthesisIndex = i;
                }
                if (condition[i] == Parenthesis.RightParenthesis)
                {
                    lastRightParenthesisIndex = i;
                    if (lastLeftParenthesisIndex == -1)
                    {
                        MessageBox.Show("左右括号不匹配");
                        return false;
                    }
                    break;
                }
            }
            if (lastLeftParenthesisIndex != -1 && lastRightParenthesisIndex == -1)
            {
                MessageBox.Show("左右括号不匹配");
                return false;
            }
            //sucess
            if (lastLeftParenthesisIndex == -1 && lastRightParenthesisIndex == -1)
            {
                if (condition[condition.Count - 1] is LogicalOperator)
                {
                    MessageBox.Show("最后一项为逻辑运算符，请确保输入正确");
                    return false;
                }
                return true;
            }

            //remove the parentheris
            ///left 
            if (lastLeftParenthesisIndex > 0)
            {
                IBooleanUnit lastLeftOutputUnit = condition[lastLeftParenthesisIndex - 1];
                if (lastLeftOutputUnit == LogicalOperator.Neg)
                {
                    NegParenthesis(condition, ref lastLeftParenthesisIndex, ref lastRightParenthesisIndex);
                    System.Console.WriteLine("！左乘括弧");
                    Log("Evaluate !(abc..) expression");
                    System.Console.WriteLine(cv.Convert(condition, null, null, null));
                    Log(cv.Convert(condition, null, null, null).ToString());
                }
                else if (lastLeftOutputUnit == LogicalOperator.And)
                {
                    AndParethesis_Left(condition, ref lastLeftParenthesisIndex, ref lastRightParenthesisIndex);
                    System.Console.WriteLine("左乘括弧");
                    Log("Evaluate A(B+C...) expression");
                    System.Console.WriteLine(cv.Convert(condition, null, null, null));
                    Log(cv.Convert(condition, null, null, null).ToString());
                }
            }


            ///right
            if (lastRightParenthesisIndex < condition.Count - 1)
            {
                IBooleanUnit nextRightOutputUnit = condition[lastRightParenthesisIndex + 1];
                if (nextRightOutputUnit == LogicalOperator.And)
                {
                    if (!AndParethesis_Right(condition, ref lastLeftParenthesisIndex, ref lastRightParenthesisIndex))
                    {
                        return false;
                    }
                    else
                    {
                        System.Console.WriteLine("右乘括弧");
                        Log("Evaluate (B+C...)A expression");
                        System.Console.WriteLine(cv.Convert(condition, null, null, null));
                        Log(cv.Convert(condition, null, null, null).ToString());
                    }
                }

            }

            //remove
            bool toRemove = true;

            if ((lastLeftParenthesisIndex > 0 && condition[lastLeftParenthesisIndex - 1] == LogicalOperator.And) ||
                (lastLeftParenthesisIndex > 0 && condition[lastLeftParenthesisIndex - 1] == LogicalOperator.Neg))
            {
                toRemove = false;
            }
            if (lastRightParenthesisIndex < condition.Count - 1 && condition[lastRightParenthesisIndex - 1] == LogicalOperator.And)
            {
                toRemove = false;
            }
            if (toRemove)
            {
                System.Console.WriteLine("删除左右括弧");
                Log("DEL left and right parenthesises");
                condition.RemoveAt(lastLeftParenthesisIndex); lastRightParenthesisIndex--;
                condition.RemoveAt(lastRightParenthesisIndex);
            }

            //re
            if (simplify(condition))
                return true;
            else
            {
                return false;
            }
        }

        static void simplify_PostProcess(ObservableCollection<IBooleanUnit> condition)
        {
            //remove same item in little units
            //i.e. AABC+BCBD=ABC+BCD
            IList<IBooleanUnit> temp = new List<IBooleanUnit>();
            IList<int> toRemove = new List<int>();
            int i = 0;
            while (i < condition.Count)
            {
                if (condition[i] is BooleanVeriable)
                {
                    if (temp.Contains(condition[i]))
                    {
                        toRemove.Add(i);
                    }
                    else
                    {
                        temp.Add(condition[i]);
                    }
                }
                else if (condition[i] == LogicalOperator.Or)
                {
                    temp.Clear();
                }
                i++;
            }
            if (toRemove.Count > 0)
            {
                System.Console.WriteLine("去除最小项中的重复项");
                Log("Del redundent veriables in term");
                int ii = 0;
                foreach (var removeI in toRemove)
                {
                    condition.RemoveAt(removeI - ii);
                    if (condition[removeI - ii] == LogicalOperator.And)
                    {
                        condition.RemoveAt(removeI - ii);
                    }
                    else if (condition[removeI - ii - 1] == LogicalOperator.And)
                    {
                        condition.RemoveAt(removeI - ii - 1);
                    }
                    else
                    {
                        System.Diagnostics.Debug.Assert(false, "we should not come here!");
                    }
                    ii += 2;
                }
                BooleanExpressionToStringConverter cv = new BooleanExpressionToStringConverter();
                System.Console.WriteLine(cv.Convert(condition, null, null, null));
                Log(cv.Convert(condition, null, null, null).ToString());
            }
        }
        public static bool SimplifyExpriession(ObservableCollection<IBooleanUnit> condition)
        {
            if (simplify(condition))
            {
                simplify_PostProcess(condition);
                return true;
            }
            return false;
        }
    }
}
