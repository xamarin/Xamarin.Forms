using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
    [Category(UITestCategories.DatePicker)]
#endif
    [Preserve(AllMembers = true)]
    [Issue(IssueTracker.None, 88888888, "DateTime Localization Issue",
        PlatformAffected.iOS | PlatformAffected.Android)]
    public class DateTimeLocalizationTests : TestNavigationPage
    {
        protected override void Init()
        {
#if APP
			PushAsync(new GalleryPages.DateTimePickerGalleries.DateTimePickerGallery());
#endif
        }

#if UITEST
        // TimePicker Format String: H, m, s, t
        // TimePicker Time: 12 AM, 0 minute
        // Separator: ".", " "
        [Test]
        public void TimePicker24HMidnight()
        {
            RunningApp.Tap(x => x.Marked("TimePicker"));
            RunningApp.ClearText("timeFormatString");
            RunningApp.EnterText("timeFormatString", "H.m.s t");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingTime");
            RunningApp.EnterText("settingTime", "0, 0");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("timeClockOptions")[0].Text;
            Assert.AreEqual("0.0.0 A", text);
        }

        // TimePicker Format String: hh, m, tt
        // TimePicker Time: 12 AM, 0 minute
        // Separator: "/", " "
        [Test]
        public void TimePicker12HMidnight()
        {
            RunningApp.Tap(x => x.Marked("TimePicker"));
            RunningApp.ClearText("timeFormatString");
            RunningApp.EnterText("timeFormatString", "hh m/tt");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingTime");
            RunningApp.EnterText("settingTime", "0, 0");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("timeClockOptions")[0].Text;
            Assert.AreEqual("12 0/AM", text);
        }

        // TimePicker Format String: HH, mm, tt
        // TimePicker Time: 1-11 PM, 1-58 minute (1)
        // Separator: ":", " "
        [Test]
        public void TimePicker24HAfternoon()
        {
            RunningApp.Tap(x => x.Marked("TimePicker"));
            RunningApp.ClearText("timeFormatString");
            RunningApp.EnterText("timeFormatString", "HH:mm tt");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingTime");
            RunningApp.EnterText("settingTime", "13, 5");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("timeClockOptions")[0].Text;
            Assert.AreEqual("13:05 PM", text);
        }

        // TimePicker Format String: h, ss
        // TimePicker Time: 1-11 PM, 59 minute (11)
        // Separator: "-"
        [Test]
        public void TimePicker12HAfternoon()
        {
            RunningApp.Tap(x => x.Marked("TimePicker"));
            RunningApp.ClearText("timeFormatString");
            RunningApp.EnterText("timeFormatString", "h-ss");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingTime");
            RunningApp.EnterText("settingTime", "23, 59");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("timeClockOptions")[0].Text;
            Assert.AreEqual("11-00", text);
        }

        // TimePicker Format String: hh
        // TimePicker Time: 12 PM, 1-58 minute
        // Separator: none
        [Test]
        public void TimePicker12HNoon()
        {
            RunningApp.Tap(x => x.Marked("TimePicker"));
            RunningApp.ClearText("timeFormatString");
            RunningApp.EnterText("timeFormatString", "hh");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingTime");
            RunningApp.EnterText("settingTime", "12, 37");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("timeClockOptions")[0].Text;
            Assert.AreEqual("12", text);
        }

        // TimePicker Format String: HH, tt
        // TimePicker Time: 12 PM, 0 minute
        // Separator: " "
        [Test]
        public void TimePicker24HNoon()
        {
            RunningApp.Tap(x => x.Marked("TimePicker"));
            RunningApp.ClearText("timeFormatString");
            RunningApp.EnterText("timeFormatString", "HH tt");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingTime");
            RunningApp.EnterText("settingTime", "12, 0");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("timeClockOptions")[0].Text;
            Assert.AreEqual("12 PM", text);
        }

        // TimePicker Format String: hh, mm
        // TimePicker Time: 1-11 AM, 59 minute
        // Separator: ":"
        [Test]
        public void TimePicker12HMorning()
        {
            RunningApp.Tap(x => x.Marked("TimePicker"));
            RunningApp.ClearText("timeFormatString");
            RunningApp.EnterText("timeFormatString", "hh:mm");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingTime");
            RunningApp.EnterText("settingTime", "7, 59");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("timeClockOptions")[0].Text;
            Assert.AreEqual("07:59", text);
        }

        // TimePicker Format String: H
        // TimePicker Time: 1-11 AM, 1-58 minute
        // Separator: "."
        [Test]
        public void TimePicker24HMorning()
        {
            RunningApp.Tap(x => x.Marked("TimePicker"));
            RunningApp.ClearText("timeFormatString");
            RunningApp.EnterText("timeFormatString", "H.");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingTime");
            RunningApp.EnterText("settingTime", "5, 1");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("timeClockOptions")[0].Text;
            Assert.AreEqual("5.", text);
        }

        // TimePicker Format String: mm t
        // TimePicker Time: 1 PM, 59 minute
        // Separator: ","
        [Test]
        public void TimePickerNoHour()
        {
            RunningApp.Tap(x => x.Marked("TimePicker"));
            RunningApp.ClearText("timeFormatString");
            RunningApp.EnterText("timeFormatString", "mm, t");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingTime");
            RunningApp.EnterText("settingTime", "13, 59");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("timeClockOptions")[0].Text;
            Assert.AreEqual("59, P", text);
        }

        // TimePicker Format String: first half of uppercase letters
        // TimePicker Time: 11 PM, 59 minute
        [Test]
        public void TimePickerRandomUppercaseBegin()
        {
            RunningApp.Tap(x => x.Marked("TimePicker"));
            RunningApp.ClearText("timeFormatString");
            RunningApp.EnterText("timeFormatString", "ABCDEGIJLNOP");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingTime");
            RunningApp.EnterText("settingTime", "23, 59");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("timeClockOptions")[0].Text;
            Assert.AreEqual("ABCDEGIJLNOP", text);
        }

        // TimePicker Format String: latter half of uppercase letters
        // TimePicker Time: 11 PM, 59 minute
        [Test]
        public void TimePickerRandomUppercaseEnd()
        {
            RunningApp.Tap(x => x.Marked("TimePicker"));
            RunningApp.ClearText("timeFormatString");
            RunningApp.EnterText("timeFormatString", "QRSTUVWXYZ");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingTime");
            RunningApp.EnterText("settingTime", "23, 59");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("timeClockOptions")[0].Text;
            Assert.AreEqual("QRSTUVWXYZ", text);
        }

        // TimePicker Format String: other lowercase letters
        // TimePicker Time: 11 PM, 59 minute
        [Test]
        public void TimePickerRandomLowercaseLetters()
        {
            RunningApp.Tap(x => x.Marked("TimePicker"));
            RunningApp.ClearText("timeFormatString");
            RunningApp.EnterText("timeFormatString", "abceijklnopqruvwx");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingTime");
            RunningApp.EnterText("settingTime", "23, 59");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("timeClockOptions")[0].Text;
            Assert.AreEqual("abceijklnopqruvwx", text);
        }

        // DatePicker Format String: d, M, y
        // Separator: "/"
        [Test]
        public void DatePickerOneDigit()
        {
            RunningApp.Tap(x => x.Marked("DatePicker"));
            RunningApp.ClearText("dateFormatString");
            RunningApp.EnterText("dateFormatString", "d/M/y");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingDate");
            RunningApp.EnterText("settingDate", "1999, 1, 31");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("dateCalendarOptions")[0].Text;
            Assert.AreEqual("31/1/99", text);
        }

        // DatePicker Format String: dd, MM, yy
        // Separator: "-"
        [Test]
        public void DatePickerTwoDigit()
        {
            RunningApp.Tap(x => x.Marked("DatePicker"));
            RunningApp.ClearText("dateFormatString");
            RunningApp.EnterText("dateFormatString", "MM-dd-yy");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingDate");
            RunningApp.EnterText("settingDate", "2000, 2, 29");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("dateCalendarOptions")[0].Text;
            Assert.AreEqual("02-29-00", text);
        }

        // DatePicker Format String: ddd, MMM, yyy
        // Separator: ","
        [Test]
        public void DatePickerThreeDigit()
        {
            RunningApp.Tap(x => x.Marked("DatePicker"));
            RunningApp.ClearText("dateFormatString");
            RunningApp.EnterText("dateFormatString", "yyy, MMM, ddd");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingDate");
            RunningApp.EnterText("settingDate", "2010, 4, 15");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("dateCalendarOptions")[0].Text;
            Assert.AreEqual("2010, Apr, Thu", text);
        }

        // DatePicker Format String: dddd, MMMM, yyyy
        // Separator: "."
        [Test]
        public void DatePickerFourDigit()
        {
            RunningApp.Tap(x => x.Marked("DatePicker"));
            RunningApp.ClearText("dateFormatString");
            RunningApp.EnterText("dateFormatString", "MMMM.dddd.yyyy");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingDate");
            RunningApp.EnterText("settingDate", "2015, 8, 1");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("dateCalendarOptions")[0].Text;
            Assert.AreEqual("August.Saturday.2015", text);
        }

        // DatePicker Format String: MMMM, yy
        // Separator: " "
        [Test]
        public void DatePickerNoDay()
        {
            RunningApp.Tap(x => x.Marked("DatePicker"));
            RunningApp.ClearText("dateFormatString");
            RunningApp.EnterText("dateFormatString", "MMMM yy");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingDate");
            RunningApp.EnterText("settingDate", "1997, 10, 30");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("dateCalendarOptions")[0].Text;
            Assert.AreEqual("October 97", text);
        }

        // DatePicker Format String: dddd
        // Separator: none
        [Test]
        public void DatePickerDay()
        {
            RunningApp.Tap(x => x.Marked("DatePicker"));
            RunningApp.ClearText("dateFormatString");
            RunningApp.EnterText("dateFormatString", "dddd");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingDate");
            RunningApp.EnterText("settingDate", "2020, 7, 20");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("dateCalendarOptions")[0].Text;
            Assert.AreEqual("Monday", text);
        }

        // DatePicker Format String: MMM, yyyy
        // Separator: " "
        [Test]
        public void DatePickerColon()
        {
            RunningApp.Tap(x => x.Marked("DatePicker"));
            RunningApp.ClearText("dateFormatString");
            RunningApp.EnterText("dateFormatString", "yyyy: MMM");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingDate");
            RunningApp.EnterText("settingDate", "2002, 12, 31");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("dateCalendarOptions")[0].Text;
            Assert.AreEqual("2002: Dec", text);
        }

        // TimePicker Format String: first half of uppercase letters
        // TimePicker Time: 11 PM, 59 minute
        [Test]
        public void DatePickerRandomUppercaseBegin()
        {
            RunningApp.Tap(x => x.Marked("DatePicker"));
            RunningApp.ClearText("dateFormatString");
            RunningApp.EnterText("dateFormatString", "ABCDEGIJLNOP");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingDate");
            RunningApp.EnterText("settingDate", "2002, 12, 31");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("dateCalendarOptions")[0].Text;
            Assert.AreEqual("ABCDEGIJLNOP", text);
        }

        // TimePicker Format String: latter half of uppercase letters
        // TimePicker Time: 11 PM, 59 minute
        [Test]
        public void DatePickerRandomUppercaseEnd()
        {
            RunningApp.Tap(x => x.Marked("DatePicker"));
            RunningApp.ClearText("dateFormatString");
            RunningApp.EnterText("dateFormatString", "QRSTUVWXYZ");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingDate");
            RunningApp.EnterText("settingDate", "2002, 12, 31");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("dateCalendarOptions")[0].Text;
            Assert.AreEqual("QRSTUVWXYZ", text);
        }

        // TimePicker Format String: other lowercase letters
        // TimePicker Time: 11 PM, 59 minute
        [Test]
        public void DatePickerRandomLowercaseLetters()
        {
            RunningApp.Tap(x => x.Marked("DatePicker"));
            RunningApp.ClearText("dateFormatString");
            RunningApp.EnterText("dateFormatString", "abceijklnopqruvwx");
            RunningApp.PressEnter();
            RunningApp.ClearText("settingDate");
            RunningApp.EnterText("settingDate", "2002, 12, 31");
            RunningApp.PressEnter();
            var text = RunningApp.WaitForElement("dateCalendarOptions")[0].Text;
            Assert.AreEqual("abceijklnopqruvwx", text);
        }
#endif
    }
}