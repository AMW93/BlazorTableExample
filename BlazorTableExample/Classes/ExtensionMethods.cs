
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace BlazorTableExample
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Returns a delimited string from a list of strings based on the separator provided. 
        /// This is an extension method for string.Join()
        /// </summary>
        /// <param name="items"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinWith<T>(this IReadOnlyCollection<T> items, char separator)
        {
            return string.Join(separator, items.ToArray());
        }

        /// <summary>
        /// Returns a delimited string from a list of strings based on the separator provided. 
        /// This is an extension method for string.Join()
        /// </summary>
        /// <param name="items"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinWith<T>(this IEnumerable<T> items, char separator)
        {
            return string.Join(separator, items.ToArray());
        }

        /// <summary>
        /// Returns a delimited string from a list of strings based on the separator provided. 
        /// This is an extension method for string.Join()
        /// </summary>
        /// <param name="items"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinWith<T>(this IEnumerable<T> items, string separator)
        {
            return string.Join(separator, items.ToArray());
        }

        /// <summary>
        /// Returns a delimited string from a list of strings based on the separator provided. 
        /// This is an extension method for string.Join()
        /// </summary>
        /// <param name="items"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinWith<T>(this IReadOnlyCollection<T> items, string separator)
        {
            return string.Join(separator, items.ToArray());
        }

        /// <summary>
        /// Simpler and more readable way to add an item to the front of the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <param name="item"></param>
        public static void AddToFront<T>(this List<T> lst, T item)
        {
            if (lst is null)
                throw new ArgumentNullException("Can't add items to null List");

            lst.Insert(0, item);
        }

        /// <summary>
        /// Mimics the SQL IN() to checks if char exists in char array.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="chars"></param>
        /// <returns>boolean true/false</returns>
        public static bool IN(this char c, params char[] chars) => chars.Contains(c);

        /// <summary>
        /// Mimics the SQL IN() to check if string contains the specified char.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="chars"></param>
        /// <returns>boolean true/false</returns>
        public static bool IN(this char c, string str) => str.Contains(c);

        /// <summary>
        /// Mimics the SQL IN() to check if int[] contains any int of the input integer.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="ints"></param>
        /// <returns>boolean true/false</returns>
        public static bool IN(this int input, params int[] ints)
        {
            for (int i = 0; i < ints.Length; i++)
            {
                if (ints.Contains(input))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Mimics the SQL IN() to check if string[] contains an exact match of the input string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strs"></param>
        /// <returns>boolean true/false</returns>
        public static bool IN(this string str, params string[] strs)
        {
            for (int i = 0; i < strs.Length; i++)
            {
                if (strs[i] == str)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Mimics the SQL LIKE() to check if string[] contains any string of the input string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strs"></param>
        /// <returns>boolean true/false</returns>
        public static bool LIKE(this string str, params string[] strs)
        {
            for (int i = 0; i < strs.Length; i++)
            {
                if (strs[i].Contains(str))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Mimics the SQL LIKE() to check if string[] contains any string of the input string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strs"></param>
        /// <returns>boolean true/false</returns>
        public static bool LIKE(this string str, string strCheck)
        {
            if (strCheck.Contains(str))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Mimics the SQL IN() to checks if char exists in char array.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="strToCheck"></param>
        /// <returns>boolean true/false</returns>
        public static bool IN(this string input, string strToCheck) => input.Contains(strToCheck);

        public static bool IN<T>(this ICollection<T> collection, T value)
        {
            HashSet<T> hash = new HashSet<T>(collection);
            for (int i = 0; i < hash.Count; i++)
            {
                if (hash.Contains(value))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Mimics the SQL IN() function
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="arrObj"></param>
        /// <returns>Boolean represent the presence of the input within the array of objects to be compared.</returns>
        public static bool IN(this object obj, params object[] arrObj)
        {
            Type a = obj.GetType();
            Type b;
            bool isPresent = false;
            for (int i = 0; i < arrObj.Length; i++)
            {
                b = arrObj[i].GetType();
                if (a != b)
                {
                    try
                    {
                        arrObj[i] = Convert.ChangeType(arrObj[i], a);
                    }
                    catch
                    {
                        throw new ArgumentException(
                            "\nType of object could not be converted to incoming type: \n" +
                            "Incoming type: " + a.FullName + "\n" +
                            "Incoming value: " + obj.ToString() + "\n" +
                            "Type To Be Converted: " + b.FullName + "\n" +
                            "Value To Be Converted: " + arrObj[i].ToString()
                            );
                    }
                }

                //Final check if the incoming object is within the object to be compared
                if (obj.Equals(arrObj[i]))
                {
                    isPresent = true;
                    break;
                }
            }

            return isPresent;
        }

        /// <summary>
        /// Mimics the SQL BETWEEN() function with integers
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>boolean true/false</returns>
        public static bool BETWEEN(this int input, int start, int end)
        {
            if (input >= start && input <= end)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Mimics the SQL BETWEEN() function with decimals
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>boolean true/false</returns>
        public static bool BETWEEN(this decimal input, decimal start, decimal end)
        {
            if (input >= start && input <= end)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Mimics the SQL BETWEEN() function with doubles
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>boolean true/false</returns>
        public static bool BETWEEN(this double input, double start, double end)
        {
            if (input >= start && input <= end)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Mimics the SQL BETWEEN() function with longs
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>boolean true/false</returns>
        public static bool BETWEEN(this long input, long start, long end)
        {
            if (input >= start && input <= end)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Mimics the SQL BETWEEN() function with shorts
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>boolean true/false</returns>
        public static bool BETWEEN(this short input, short start, short end)
        {
            if (input >= start && input <= end)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Mimics the SQL BETWEEN() function with floats
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>boolean true/false</returns>
        public static bool BETWEEN(this float input, float start, float end)
        {
            if (input >= start && input <= end)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Mimics the SQL BETWEEN() function with chars to check if a letter is between two other letters
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>boolean true/false</returns>
        public static bool BETWEEN(this char input, char start, char end)
        {
            if (char.IsLetter(input) && char.IsLetter(start) && char.IsLetter(end))
            {
                if (string.Compare(input.ToString(), start.ToString(), true) >= 0 && string.Compare(input.ToString(), end.ToString(), true) <= 0)
                    return true;
                else
                    return false;
            }
            else if (char.IsNumber(input) && char.IsNumber(start) && char.IsNumber(end))
            {
                if (input.ToInt() >= start.ToInt() && input.ToInt() <= end.ToInt())
                    return true;
                else
                    return false;
            }
            else
                throw new Exception(
                    "Extension Methods BETWEEN().\n" +
                    "Char was not a letter or number. \n" +
                    "Input char: " + input + "\n" +
                    "Start char: " + start + "\n" +
                    "End char: " + end
                    );
        }

        /// <summary>
        /// Mimics the SQL BETWEEN() function with dates to check if a date is between two other dates.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>boolean true/false</returns>
        public static bool BETWEEN(this DateTime input, DateTime start, DateTime end) => input.Ticks >= start.Ticks && input.Ticks <= end.Ticks;

        /// <summary>
        /// Mimics the SQL BETWEEN() function.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>Boolean representing the presence of the input object within the range to be compared.</returns>
        public static bool BETWEEN(this object obj, object start, object end)
        {
            Type a = obj.GetType();
            Type s = start.GetType();
            Type e = end.GetType();

            //if incoming start object is not same object type as the incoming object then try to convert them to be the same type
            if (a != s)
            {
                try
                {
                    //try to change the type of start object to type of obj object
                    start = Convert.ChangeType(start, a);
                }
                catch (Exception ee)
                {
                    Type aa = ee.GetType();
                    throw new FormatException(
                                          "\nObject could not be converted to incoming type: \n" +
                                            "Incoming type: " + a.FullName + "\n" +
                                            "Incoming value: " + obj.ToString() + "\n" +
                                            "Start Type To Be Converted: " + s.FullName + "\n" +
                                            "Start Value To Be Converted: " + start.ToString() + "\n" +
                                            "End Type To Be Converted: " + e.FullName + "\n" +
                                            "End Value To Be Converted: " + end.ToString()
                        );
                }
            }

            if (a != e)
            {
                try
                {   //try to change the type of end object to type of obj object
                    end = Convert.ChangeType(end, a);
                }
                catch (Exception)
                {
                    throw new FormatException(
                                          "\nObject could not be converted to incoming type: \n" +
                                            "Incoming type: " + a.FullName + "\n" +
                                            "Incoming value: " + obj.ToString() + "\n" +
                                            "Start Type To Be Converted: " + s.FullName + "\n" +
                                            "Start Value To Be Converted: " + start.ToString() + "\n" +
                                            "End Type To Be Converted: " + e.FullName + "\n" +
                                            "End Value To Be Converted: " + end.ToString()
                        );
                }
            }

            //Decimal is used because DateTime, long, float, double, decimal, short can all be parsed to a decimal for comaprison
            //These are the only legitimate types OASIS uses as of 2022-03-25.
            decimal finalObj;
            decimal finalStart;
            decimal finalEnd;

            //DateTime has to be parsed to a long, then to decimal.
            //Everything can be parsed straight to a decimal for comparison

            //First test if the object is a string and we'll see if the string can be converted to a DateTime.
            //Otherwise a straight conversion will work
            try
            {
                finalObj = TestObjectParse(obj);
                finalStart = TestObjectParse(start);
                finalEnd = TestObjectParse(end);
            }
            catch (Exception)
            {
                throw new FormatException(
                                      "\nObject could not be converted to incoming type: \n" +
                                        "Incoming type: " + a.FullName + "\n" +
                                        "Incoming value: " + obj.ToString() + "\n" +
                                        "Start Type To Be Converted: " + s.FullName + "\n" +
                                        "Start Value To Be Converted: " + start.ToString() + "\n" +
                                        "End Type To Be Converted: " + e.FullName + "\n" +
                                        "End Value To Be Converted: " + end.ToString()
                    );
            }

            //everything has been parsed down to a decimal by this point
            //switch the values if the start is greater than the end
            if (finalStart > finalEnd)
            {
                decimal temp = finalStart;
                finalStart = finalEnd;
                finalEnd = temp;
            }

            //This is a fancy way of doing it with a tuple
            //The compiler will break this down to using a temp variable anyways 
            //Tuples are cool but
            //if (finalStart > finalEnd)
            //    (finalStart, finalEnd) = (finalEnd, finalStart);

            //Make the check if the input is between the start and end to be compared
            if (finalObj >= finalStart && finalObj <= finalEnd)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Mimics the SQL BETWEEN() function but takes in string to better represent the SQL syntax. object values should be separated by the word 'and' or 'AND'.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="input"></param>
        /// <returns>Boolean representing the presence of the input object within the range to be compared.</returns>
        public static bool BETWEEN(this object obj, string input)
        {
            string[] objs;
            try
            {
                objs = input.Split(new string[] { "and" }, StringSplitOptions.None);
            }
            catch (ArgumentException)
            {
                throw new FormatException(
                                             "\nIncoming text did not contain the word 'and' and/or could not be split into a string array. \n" +
                                             "Incoming string: " + input
                           );

            }

            //Must contain 2 objects in the array representing a start and end point to check against the input
            if (objs.Length != 2)
            {
                throw new ArgumentException("\nInput object improper length for BETWEEN(this object obj, string input). \nInput object: " + obj.ToString() + "\nInput string: " + input);
            }


            object start = objs[0].Trim();
            object end = objs[1].Trim();

            Type a = obj.GetType();
            Type s = start.GetType();
            Type e = end.GetType();

            //if incoming start object is not same object type as the incoming object then try to convert them to be the same type
            if (a != s)
            {
                try
                {
                    //try to change the type of start object to type of obj object
                    start = Convert.ChangeType(start, a);
                }
                catch (Exception)
                {
                    throw new FormatException(
                                          "\nObject could not be converted to incoming type: \n" +
                                            "Incoming type: " + a.FullName + "\n" +
                                            "Incoming value: " + obj.ToString() + "\n" +
                                            "Start Type To Be Converted: " + s.FullName + "\n" +
                                            "Start Value To Be Converted: " + start.ToString() + "\n" +
                                            "End Type To Be Converted: " + e.FullName + "\n" +
                                            "End Value To Be Converted: " + end.ToString()
                        );
                }
            }

            if (a != e)
            {
                try
                {   //try to change the type of end object to type of obj object
                    end = Convert.ChangeType(end, a);
                }
                catch (Exception)
                {
                    throw new FormatException(
                                          "\nObject could not be converted to incoming type: \n" +
                                            "Incoming type: " + a.FullName + "\n" +
                                            "Incoming value: " + obj.ToString() + "\n" +
                                            "Start Type To Be Converted: " + s.FullName + "\n" +
                                            "Start Value To Be Converted: " + start.ToString() + "\n" +
                                            "End Type To Be Converted: " + e.FullName + "\n" +
                                            "End Value To Be Converted: " + end.ToString()
                        );
                }
            }

            //Decimal is used because DateTime, long, float, double, decimal, short can all be parsed to a decimal for comaprison
            //These are the only legitimate types OASIS uses as of 2022-03-25.
            decimal finalObj;
            decimal finalStart;
            decimal finalEnd;

            //DateTime has to be parsed to a long, then to decimal.
            //Everything can be parsed straight to a decimal for comparison

            //First test if the object is a string and we'll see if the string can be converted to a DateTime.
            //Otherwise a straight conversion will work
            try
            {
                finalObj = TestObjectParse(obj);
                finalStart = TestObjectParse(start);
                finalEnd = TestObjectParse(end);
            }
            catch (Exception)
            {
                throw new FormatException(
                                      "\nObject could not be converted to incoming type: \n" +
                                        "Incoming type: " + a.FullName + "\n" +
                                        "Incoming value: " + obj.ToString() + "\n" +
                                        "Start Type To Be Converted: " + s.FullName + "\n" +
                                        "Start Value To Be Converted: " + start.ToString() + "\n" +
                                        "End Type To Be Converted: " + e.FullName + "\n" +
                                        "End Value To Be Converted: " + end.ToString()
                    );
            }

            //everything has been parsed down to a decimal by this point
            //switch the values if the start is greater than the end
            if (finalStart > finalEnd)
            {
                decimal temp = finalStart;
                finalStart = finalEnd;
                finalEnd = temp;
            }

            //This is a fancy way of doing it with a tuple
            //The compiler will break this down to using a temp variable anyways 
            //Tuples are cool but
            //if (finalStart > finalEnd)
            //    (finalStart, finalEnd) = (finalEnd, finalStart);

            //Make the check if the input is between the start and end to be compared
            if (finalObj >= finalStart && finalObj <= finalEnd)
                return true;
            else
                return false;
        }

        private static decimal TestObjectParse(object obj)
        {
            if (DateTime.TryParseExact(obj.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                return Convert.ToDecimal(date.Ticks);
            else if (obj.GetType() == typeof(DateTime))
                return Convert.ToDecimal(Convert.ToDateTime(obj).Ticks);
            else
                return Convert.ToDecimal(obj);
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                DataColumn column = dr.Table.Columns[i];
                for (int j = 0; j < temp.GetProperties().Length; j++)
                {
                    PropertyInfo pro = temp.GetProperties()[i];
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName] == DBNull.Value ? GetProperType(column) : dr[column.ColumnName], null);
                    else
                        continue; //Transfer all power from life support to main thrusters.
                }

            }
            return obj;
        }

        /// <summary>
        /// Return the left side of a string.Split() operation. This is just an easier way to read string.Split(separator)[0];
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        public static string Left(this string[] str) => str[0];

        /// <summary>
        /// Simpler way of writing a string.Split() where the separator and left of separator is returned is in one function call.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator"></param>
        /// <returns>string</returns>
        public static string LeftOf(this string str, char separator) => str.Split(separator)[0];

        /// <summary>
        /// Simpler way of writing a string.Split() where the separator and right of separator is returned is in one function call.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator"></param>
        /// <returns>string</returns>
        public static string RightOf(this string str, char separator) => str.Split(separator)[1];

        /// <summary>
        /// Return the right side of a string.Split() operation. This is just an easier way to read string.Split(separator)[1].
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        public static string Right(this string[] str) => str[1];

        /// <summary>
        /// Capitalizes the first letter of a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        public static string CapitalizeFirst(this string str)
        {
            switch (str)
            {
                case null: throw new ArgumentNullException(nameof(str));
                case "": throw new ArgumentException($"{nameof(str)} cannot be empty", nameof(str));
                default: return str[0].ToString().ToUpper() + str.Substring(1);
            }
        }

        /// <summary>
        /// Capitalizes a character at the specified index within a string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pos"></param>
        /// <returns>string</returns>
        public static string Capitaize(this string str, int pos)
        {
            char[] chars = str.ToCharArray();

            try
            {
                for (int i = 0; i < chars.Length; i++)
                {
                    if (i == pos)
                        chars[i] = char.ToUpper(chars[i]);
                }
                return new string(chars);
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException(
                    "ExtensionMethods Capitalize(this string str, int pos)... \n" +
                    "Index out of range of char[]. \n" +
                    "Index: " + pos.ToString() + "\n" +
                    "string: " + str + "\n" +
                    "char[] size" + chars.Length.ToString()
                    );
            }
        }

        public static string[] CapitalizeFirst(this string[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                string str = input[i];
                switch (str)
                {
                    case null:
                        throw new ArgumentNullException(nameof(str));
                    case "":
                        throw new ArgumentException($"{nameof(str)} cannot be empty", nameof(str));
                    default:
                        input[i] = str[0].ToString().ToUpper() + str.Substring(1);
                        break;
                }
            }

            return input;
        }

        /// <summary>
        /// Simpler way to check if a string is null, empty, or exlusively containing white space characters. 
        /// </summary>
        /// <param name="str"></param>
        /// <returns>boolean true/false</returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return str == null || str.Trim() == string.Empty;
        }

        /// <summary>
        /// When the GetItem function gets returned a null for whatever reason it would crash.
        /// You cannot assign null to a non nullable data type. 
        /// This is a simple way to give some default value when DBNull.Value is present in a DataRow
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        private static object GetProperType(DataColumn col)
        {
            object obj = "";
            switch (col.DataType.Name.ToLower())
            {
                case "string":
                    obj = "";
                    break;
                case "int":
                    obj = 0;
                    break;
                case "byte":
                    obj = new byte[0];
                    break;
                case "bool":
                    obj = false;
                    break;
                case "long":
                    obj = 0L;
                    break;
                case "decimal":
                    obj = 0.0M;
                    break;
                case "datetime":
                    obj = Convert.ToDateTime("1900-01-01");
                    break;
                case "guid":
                    obj = Guid.Empty;
                    break;
            }
            return obj;
        }

        static GregorianCalendar _gc = new GregorianCalendar();

        public static int GetWeekOfMonth(this DateTime time)
        {
            DateTime first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        static int GetWeekOfYear(this DateTime time)
        {
            return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        public static int ToInt(this char c)
        {
            if (char.IsNumber(c))
                return Convert.ToInt32(c);
            else
            {
                throw new Exception(
                    "Extension Methods... ToInt(this char c)\n" +
                    "Char was not a number: \'" + c + "\'"
                    );
            }
        }

        /// <summary>
        /// Converts DataKey to integer. Extension method for quicker writing.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>Integer</returns>
        public static int ToInt(this ValueType valueType) => Convert.ToInt32(valueType);

        /// <summary>
        /// Converts string to integer. Extension method for quicker writing.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>Integer</returns>
        public static int ToInt(this string str) => Convert.ToInt32(str);

        /// <summary>
        /// Convert string to long. Extension method for quicker writing.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>long</returns>
        public static long ToLong(this string str) => Convert.ToInt64(str);

        /// <summary>
        /// Converts string to decmal. Extension method for quicker writing.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>decimal</returns>
        public static decimal ToDecimal(this string str) => Convert.ToDecimal(str);

        /// <summary>
        /// Convert string to double. Extension method for quicker writing.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>doubles</returns>
        public static double ToDouble(this string str) => Convert.ToDouble(str);

        /// <summary>
        /// Convert string to float. Extension method for quicker writing.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>float</returns>
        public static float ToFloat(this string str) => Convert.ToSingle(str);

        /// <summary>
        /// Convert string to short. Extension method for quicker writing.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>short</returns>
        public static float ToShort(this string str) => Convert.ToInt16(str);

        /// <summary>
        /// Convert string to money string. Extension method for quicker writing.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string formatted as money</returns>
        public static string ToMoney(this string str) => !str.IsNullOrEmpty() ? string.Format("{0:C2}", Convert.ToDecimal(str) / 100) : "";

        /// <summary>
        /// Convert decimal to money string. Extension method for quicker writing.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string formatted as money</returns>
        public static string ToMoney(this decimal dec) => string.Format("{0:C2}", dec);

        /// <summary>
        /// Converts date string to string of a different DateTime format. Extension method for quicker writing.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="formatting"></param>
        /// <returns>DateTime string with specified formatting</returns>
        public static string ToDateString(this string str, string formatting) => !str.IsNullOrEmpty() ? Convert.ToDateTime(str).ToString(formatting) : "";

        /// <summary>
        /// Converts DateTime to string of a different DateTime format. Extension method for linearity with other extension method 'ToDateString()'.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="formatting"></param>
        /// <returns>DateTime string formatted as 'yyyy-MM-dd'</returns>
        public static string ToDateString(this DateTime dateTime, string formatting) => dateTime.ToString(formatting);

        /// <summary>
        /// Converts date string to format 'yyyy'MM'dd' for textboxes with TextMode='Date' attribute. Extension method for quicker writing.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string formatted as 'yyyy-MM-dd'</returns>
        public static string ToTextModeDate(this string str) => !str.IsNullOrEmpty() ? Convert.ToDateTime(str).ToString("yyyy-MM-dd") : "";

        /// <summary>
        /// Converts DateTime to string with format 'yyyy'MM'dd' for textboxes with TextMode='Date' attribute. Extension method for quicker writing.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string formatted as 'yyyy-MM-dd'</returns>
        public static string ToTextModeDate(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd");

        /// <summary>
        /// Converts string to DateTime. Extension method for quicker writing.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>DateTime</returns>
        public static DateTime ToDate(this string str) => Convert.ToDateTime(str);

        /// <summary>
        /// Converts SSN string to properly formatted SSN. Extension method for quicker writing.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        public static string ToSSN(this string str) => !str.IsNullOrEmpty() ? string.Format("{0:000-00-0000}", Convert.ToInt64(str)) : "";

        /// <summary>
        /// Converts phone number string to properly formatted phone number. Extension method for quicker writing.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        public static string ToPhone(this string str) => !str.IsNullOrEmpty() ? string.Format("{0:(###) ###-####}", Convert.ToInt64(str)) : "";

        /// <summary>
        /// Converts phone number string to properly formatted phone number. Extension method for quicker writing.
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns>string</returns>
        public static string ToPhone(this long phoneNum) => string.Format("{0:(###) ###-####}", phoneNum);

        /// <summary>
        /// Convert integer to formatted percentage string. Extension method for quicker writing
        /// </summary>
        /// <param name="num"></param>
        /// <returns>string</returns>
        public static string ToPercent(this int num) => string.Format("{0}%", num.ToString());

        /// <summary>
        /// Convert string to formatted percentage string. Extension method for quicker writing
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        public static string ToPercent(this string str) => !str.IsNullOrEmpty() ? string.Format("{0}%", str) : "";

        /// <summary>
        /// Convert double to formatted percentage string. Extension method for quicker writing
        /// </summary>
        /// <param name="dbl"></param>
        /// <returns>string</returns>
        public static string ToPercent(this double dbl) => dbl.ToString(dbl.ToString("P", CultureInfo.CreateSpecificCulture("hr-HR")));

        /// <summary>
        /// Convert decimal to formatted percentage string. Extension method for quicker writing
        /// </summary>
        /// <param name="dcml"></param>
        /// <returns>string</returns>
        public static string ToPercent(this decimal dcml) => dcml.ToString(dcml.ToString("P", CultureInfo.CreateSpecificCulture("hr-HR")));

        /// <summary>
        /// Convert byte[] to base 64 encoded string. Extension method for quicker writing
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToBase64(this byte[] bytes) => Convert.ToBase64String(bytes);

        /// <summary>
        /// Converts string to byte[]. Extension method for quicker writing. 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] FromBase64(this string str) => Convert.FromBase64String(str);

        /// <summary>
        /// Checks if given class object has specified property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns>boolean true/false</returns>
        public static bool HasProperty(this object obj, string propertyName) => obj.GetType().GetProperty(propertyName) != null;

        /// <summary>
        /// Adds range of items to a list easier. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="list"></param>
        /// <param name="values"></param>
        /// <returns>nada</returns>
        public static void AddRange<T, S>(this IList<T> list, params S[] values) where S : T
        {
            for (int i = 0; i < values.Length; i++)
            {
                S value = values[i];
                list.Add(value);
            }
        }

    }
}
