//
// Please make sure to read and understand the files README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 3/2019
// Last modified: 4/2020

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cocop.MessageSerialiser.Biz.Neutral;

namespace HelperTest
{
    [TestClass]
    public class UnitTest1
    {
        // Not testing all of the helper, just parts of it.

        [TestMethod]
        public void DoubleToString()
        {
            // To test "double to string", looked up possible outcomes at:
            // http://books.xmlschemata.org/relaxng/ch19-77065.html
            //
            // Eric van der Vlist: RELAX NG
            // Released December 2003
            // Publisher(s): O'Reilly Media, Inc.
            // ISBN: 0596004214
            Assert.AreEqual("0", Helper.DoubleToString(0));
            Assert.AreEqual("10.1", Helper.DoubleToString(10.1));
            Assert.AreEqual("-3.8", Helper.DoubleToString(-3.8));
            Assert.AreEqual("2E+15", Helper.DoubleToString(2e15));
            Assert.AreEqual("NaN", Helper.DoubleToString(double.NaN));
        }

        [TestMethod]
        public void DoubleFromString()
        {
            // 1) Positive testing
            Assert.AreEqual(0, Helper.DoubleFromString("0 "), 0.001);
            Assert.AreEqual(10.1, Helper.DoubleFromString(" 10.1"), 0.001);
            Assert.AreEqual(-3.8, Helper.DoubleFromString("-3.8  "), 0.001);
            Assert.AreEqual(9e15, Helper.DoubleFromString("9e15"), 0.001);
            Assert.IsTrue(double.IsNaN(Helper.DoubleFromString("  NaN")));

            // 2) Negative testing
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.DoubleFromString("0,4");
            },
            "Failed to parse double");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.DoubleFromString("");
            },
            "Failed to parse double");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.DoubleFromString("  ");
            },
            "Failed to parse double");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.DoubleFromString("a");
            },
            "Failed to parse double");
        }

        [TestMethod]
        public void LongToString()
        {
            Assert.AreEqual("0", Helper.LongToString(0));
            Assert.AreEqual("39", Helper.LongToString(39));
            Assert.AreEqual("-8", Helper.LongToString(-8));
            Assert.AreEqual("-9223372036854775808", Helper.LongToString(-9223372036854775808));
            Assert.AreEqual("9223372036854775807", Helper.LongToString(9223372036854775807));
        }

        [TestMethod]
        public void LongFromString()
        {
            // 1) Positivite testing
            Assert.AreEqual(0, Helper.LongFromString("0  "));
            Assert.AreEqual(39, Helper.LongFromString("  39"));
            Assert.AreEqual(-8, Helper.LongFromString("-8 "));
            Assert.AreEqual(-9223372036854775808, Helper.LongFromString(" -9223372036854775808")); // Min value
            Assert.AreEqual(9223372036854775807, Helper.LongFromString("9223372036854775807 ")); // Max value

            // 2) Negative testing
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.LongFromString(" "); // Empty value
            },
            "Failed to parse long");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.LongFromString("rtt"); // Invalid
            },
            "Failed to parse long");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.LongFromString("5.3"); // Invalid
            },
            "Failed to parse long");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.LongFromString("9223372036854775808"); // Too big
            },
            "Failed to parse long");
        }

        [TestMethod]
        public void IntToString()
        {
            Assert.AreEqual("0", Helper.IntToString(0));
            Assert.AreEqual("39", Helper.IntToString(39));
            Assert.AreEqual("-8", Helper.IntToString(-8));
            Assert.AreEqual("-2147483648", Helper.IntToString(-2147483648));
            Assert.AreEqual("2147483647", Helper.IntToString(2147483647));
        }

        [TestMethod]
        public void IntFromString()
        {
            // 1) Positivite testing
            Assert.AreEqual(0, Helper.IntFromString("0  "));
            Assert.AreEqual(39, Helper.IntFromString("  39"));
            Assert.AreEqual(-8, Helper.IntFromString("-8 "));
            Assert.AreEqual(-2147483648, Helper.IntFromString(" -2147483648")); // Min value
            Assert.AreEqual(2147483647, Helper.IntFromString("2147483647 ")); // Max value

            // 2) Negative testing
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.IntFromString(" "); // Empty value
            },
            "Failed to parse Int32");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.IntFromString("rtt"); // Invalid
            },
            "Failed to parse Int32");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.IntFromString("5.3"); // Invalid
            },
            "Failed to parse Int32");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.IntFromString("2147483648"); // Too big
            },
            "Failed to parse Int32");
        }

        [TestMethod]
        public void BoolToString()
        {
            Assert.AreEqual("true", Helper.BoolToString(true));
            Assert.AreEqual("false", Helper.BoolToString(false));
        }

        [TestMethod]
        public void BoolFromString()
        {
            Assert.IsTrue(Helper.BoolFromString("  1 "));
            Assert.IsTrue(Helper.BoolFromString("true  "));
            Assert.IsFalse(Helper.BoolFromString("  0"));
            Assert.IsFalse(Helper.BoolFromString(" false"));

            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.BoolFromString("fafse");
            },
            "Failed to parse boolean");
        }
        
        [TestMethod]
        public void DateTimeToUtcIfPossible()
        {
            // Testing the conversion of kind and time zone

            var dtUtc = DateTime.SpecifyKind(DateTime.Parse("2020-02-20T12:24:00"), DateTimeKind.Utc);
            var dtUnspec = DateTime.SpecifyKind(DateTime.Parse("2020-02-20T12:24:00"), DateTimeKind.Unspecified);
            var dtLocal = DateTime.SpecifyKind(DateTime.Parse("2020-02-20T12:24:00"), DateTimeKind.Local);
            var dtExotic = DateTime.Parse("2020-02-20T12:24:00-05:00");
            var localTimeZone = TimeZone.CurrentTimeZone;

            // 1) UTC kind -> "already OK"
            AssertHourAndKind(12, DateTimeKind.Utc, Helper.DateTimeToUtcIfPossible(dtUtc));

            // 2) Unspecified kind -> "do not touch"
            AssertHourAndKind(12, DateTimeKind.Unspecified, Helper.DateTimeToUtcIfPossible(dtUnspec));

            // 3) Local time -> convert to UTC
            var offsetHours = Convert.ToInt32(localTimeZone.GetUtcOffset(dtLocal).TotalHours);
            AssertHourAndKind(12 - offsetHours, DateTimeKind.Utc, Helper.DateTimeToUtcIfPossible(dtLocal));

            // 4) Local time of another time zone -> convert to UTC
            AssertHourAndKind(17, DateTimeKind.Utc, Helper.DateTimeToUtcIfPossible(dtExotic));
        }

        private void AssertHourAndKind(int expHour, DateTimeKind expKind, DateTime dt)
        {
            Assert.AreEqual(expHour, dt.Hour);
            Assert.AreEqual(expKind, dt.Kind);
        }
    }
}
