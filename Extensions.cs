﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using System.Windows.Forms;
using System.Collections;

namespace ExtensionLib
{
    public static class Extensions
    {
        

        #region String

        public static byte ToByte(this string str)
        {
            if (str.Length < 8) str = new string('0', 8 - str.Length) + str;
            return Convert.ToByte(str, 2);
        }

        public static byte[] ToByteArray(this string str)
        {
            return Enumerable.Range(0, str.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(str.Substring(x, 2), 16))
                .ToArray();
        }
        public static double ToDouble(this string str, double min = -Double.MaxValue, double max = Double.MaxValue)
        {
            str = str.Replace(",", ".");
            double d = Conversion.Val(str);
            if (d < min) d = min;
            if (d > max) d = max;
            return d;
        }
        public static decimal ToDecimal(this string str, decimal min = -decimal.MaxValue, decimal max = decimal.MaxValue)
        {
            str = str.Replace(".", ",");
            decimal d = Conversion.CTypeDynamic<decimal>(str);
            
            //decimal d = Convert.ToDecimal(str);
            if (d < min) d = min;
            if (d > max) d = max;
            return d;
        }

        public static int ToInt(this string str, double min = -Double.MaxValue, double max = Double.MaxValue)
        {
            str = str.Replace(",", ".");
            double d = Conversion.Val(str);

            if (d < min) d = min;
            if (d > max) d = max;
            return (int)d;
        }
        public static int ToIntFromHex(this string str, double min = -Double.MaxValue, double max = Double.MaxValue)
        {
            str = str.Replace(",", ".");
            double d = Convert.ToInt32(str,16);

            if (d < min) d = min;
            if (d > max) d = max;
            return (int)d;
        }
        public static string Delete(this string str, int begin, int end)
        {
            //begin = begin == 0 ? 1 : begin;
            //end = end == str.Length - 1 ? str.Length - 2 : end;
            string s = str.Substring(0, begin) + str.Substring(end, str.Length - end);
            return s;
        }
        //public static string BracketTrim(this string str, string brStart, string brEnd)
        //{
        //    string s = Regex.Match(str,@"\" + brStart + @".+?\" + brEnd).Value.TrimStart(brStart.ToCharArray()).TrimEnd(brEnd.ToCharArray());
        //    return s;
        //}

        public static string Slice(this string str, int begin, int end)
        {
            begin = begin == 0 ? 1 : begin;
            string s = str.Substring(begin - 1, end - begin + 1);
            return s;
        }
        #endregion

        #region Byte or Byte[]
        public static string ToStringHex(this byte b)
        {
            string s = b.ToString("X");
            if (s.Length == 1) s = "0" + s;
            return s;
        }
        public static string ToStringBit(this byte b)
        {
            string s = Convert.ToString(b, 2);
            if (s.Length < 8) s = new string('0', 8 - s.Length) + s;
            return s;
        }
        #endregion

        #region double
        public static double Pow2(this double d)
        {
            return Math.Pow(d, 2);
        }
        public static double Pow3(this double d)
        {
            return Math.Pow(d, 3);
        }
        public static string ToStrInv(this double d)
        {
            return d.ToString(CultureInfo.InvariantCulture);
        }

        public static double Round(this double d,int numberOfValueDigit)
        {
            if (numberOfValueDigit < 1) 
                throw new ArgumentOutOfRangeException("Количество значащих цифр не должно быть меньше 1");

            var format = $@"0.{new string('0', numberOfValueDigit-1)}E+00";
            string dStr = d.ToString(format, CultureInfo.InvariantCulture);
            double dNew = dStr.ToDouble();
            return dNew;
        }
        #endregion

        public static List<T> ScaleToPowerOfTwo<T>(this List<T> list)
        {
            int length = list.Count;
            int nextPowerOfTwo = (int)Math.Pow(2, Math.Ceiling(Math.Log(length,2)));

            if (nextPowerOfTwo == length)
            {
                // No need to scale, already a power of 2
                return list;
            }

            // Create a new list with the next power of 2 length
            List<T> scaledList = new List<T>(nextPowerOfTwo);

            // Copy the original values to the scaled list
            scaledList.AddRange(list);

            // Fill the remaining elements with zeros
            for (int i = length; i < nextPowerOfTwo; i++)
            {
                scaledList.Add(default(T));
            }

            return scaledList;
        }

        //public static IEnumerable<T> MinMaxFilter<T>(this IEnumerable<T> list,int chunkSize)
        //{
        //    var filtredData = list.Select((x, i) => new { Index = i, Value = x })
        //    .GroupBy(x => x.Index / chunkSize)
        //    .SelectMany(x => new  { x.Aggregate((i1, i2) => i1.Value.L > i2.Value.L ? i1 : i2).Value, x.Aggregate((i1, i2) => i1.Value.L < i2.Value.L ? i1 : i2).Value })
        //    .ToList();
        //}

        public static List<T> ToList<T> (this  BlockingCollection<T> bl)
        {
            List<T> l = new List<T>();
            foreach (T v in bl)
            {
                l.Add(v);
            }
            return l;
        }
        public static string ToStringHex(this int b)
        {
            string s = b.ToString("X");
            if (s.Length == 1) s = "0" + s;
            return s;
        }
        public static int ToInt(this TextBox tb, double min = -Double.MaxValue, double max = Double.MaxValue)
        {
           return tb.Text.ToInt(min, max);
        }

        #region Others
        public static string ToStr(this DateTime dt, bool msec = false)
        {
            return msec ? dt.ToString("dd.MM.yyyy HH:mm:ss_fff") : dt.ToString();
        }
        public static void ForEach<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            int i = 0;
            foreach (var e in ie)
            {
                action(e, i++);
            }
        }
        public static T MaxBy<U,T>(this IEnumerable<T> en,Func<T,U> f) where U:IComparable<U>
        {
           return en.Aggregate((w1, w2) => f(w1).CompareTo(f(w2))>0 ? w1 : w2); ;
        }
        public static T MinBy<U, T>(this IEnumerable<T> en, Func<T, U> f) where U : IComparable<U>
        {
            return en.Aggregate((w1, w2) => f(w1).CompareTo(f(w2)) < 0 ? w1 : w2); ;
        }
        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var e in ie)
            {
                action(e);
            }
        }
        public static IEnumerable<TU> USelect<T,TU>(this IEnumerable<T> ie, Func<T, int, IEnumerable<T>, TU> func)
        {
            int i = 0;

            foreach (var e in ie)
            {
                yield return func(e, i++, ie);
            }
            
        }
        public static IEnumerable<TU> USelect<T,TU>(this IEnumerable<T> ie, Func<T, int, TU> func)
        {
            int i = 0;

            foreach (var e in ie)
            {
               yield return func(e, i++);
            }
            
        }
        public static IEnumerable<TU> USelect<TU,T>(this IEnumerable<T> ie, Func<T, int, TU> func, IScipParam<TU> scipParam)
        {
            TU ReturnFunc(T e, int i1)
            {

                if (scipParam != null)
                {
                    if (scipParam.scipFirstCount != 0 && i1 < scipParam.scipFirstCount) return scipParam.returnedValue;
                    if (scipParam.scipLastCount != 0 && i1 > scipParam.scipLastCount) return scipParam.returnedValue;
                }

                return func(e, i1);
                
            }

            int i = 0;

            foreach (var e in ie)
            {
                yield return ReturnFunc(e,i++);
            }

        }
        public static void ForEach(this DataGridViewRowCollection rows, Action<DataGridViewRow, int> action)
        {
            int i = 0;
            foreach (var e in rows)
            {
                action((DataGridViewRow)e, i++);
            }
        }
        public static void AddRange(this ObservableCollection<String> oList, string[] sarr)
        {
            foreach (string s in sarr)
            {
               oList.Add(s);
            }
        }


        #endregion

        #region Object
        //public static T AsType<T>(this object a, Type b)
        //{
        //    var res = Convert.ChangeType(a, b);
        //    return T ;
        //}
        #endregion

    }

    public interface IScipParam<T>
    {
        T returnedValue { get; }

        int scipFirstCount { get; }
        int scipLastCount { get; }
    }


}
