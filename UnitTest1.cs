using NUnit.Framework;
using System;
using System.Drawing;
using System.Collections.Generic;

namespace AdvancedSoftwareEngineeringPart1.Tests
{
    [TestFixture]
    public class CommandParserTests
    {
        private CommandParser commandParser;
        private Graphics graphics;
        private Size panelSize;

        [SetUp]
        public void Setup()
        {
           // graphics = Graphics.FromImage(new Bitmap(1, 1));
            panelSize = new Size(100, 100);
           // commandParser = new CommandParser(graphics, panelSize);
        }

        [Test]
        public void ExecuteProgram_NullCommands_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => commandParser.ExecuteProgram(null));
        }

        [Test]
        public void SyntaxCheck_NullCommand_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => commandParser.SyntaxCheck(null));
        }

        [Test]
        public void SetPenColor_InvalidColor_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => commandParser.SetPenColor("invalid_color"));
        }

        [TestCase("position 10 20", true)]
        [TestCase("pen red", true)]
        [TestCase("draw 10 20", true)]
        [TestCase("clear", true)]
        [TestCase("reset", true)]
        [TestCase("rectangle 10 20", true)]
        [TestCase("circle 10", true)]
        [TestCase("triangle 0 0 10 0 10 10", true)]
        [TestCase("fill on", true)]
        [TestCase("invalid_command", false)]
        [TestCase("position 10", false)]
        [TestCase("pen", false)]
        [TestCase("draw", false)]
        [TestCase("clear ", false)]
        [TestCase("reset test", false)]
        [TestCase("rectangle 10 test", false)]
        [TestCase("circle", false)]
        [TestCase("triangle 0 0 10 0", false)]
        [TestCase("fill", false)]
        public void SyntaxCheck_ValidAndInvalidCommands_ReturnsExpectedResult(string command, bool expectedResult)
        {
            bool isValidSyntax = true;
            try
            {
                commandParser.SyntaxCheck(command);
            }
            catch (ArgumentException)
            {
                isValidSyntax = false;
            }
            Assert.AreEqual(expectedResult, isValidSyntax);
        }

        [Test]
        public void IsValidColor_ValidColors_ReturnsTrue()
        {
            Assert.IsTrue(commandParser.IsValidColor("red"));
            Assert.IsTrue(commandParser.IsValidColor("green"));
            Assert.IsTrue(commandParser.IsValidColor("blue"));
            Assert.IsTrue(commandParser.IsValidColor("black"));
        }
        [Test]
        public void SyntaxCheck_ValidAndInvalidCommands_ReturnsExpectedResult()
        {
            string command = "position 10 20";
            bool expectedResult = true;

            bool isValidSyntax = true;
            try
            {
                commandParser.SyntaxCheck(command);
            }
            catch (ArgumentException)
            {
                isValidSyntax = false;
            }

            Assert.That(isValidSyntax, Is.EqualTo(expectedResult));
        }


        [Test]
        public void IsValidColor_InvalidColor_ReturnsFalse()
        {
            Assert.IsFalse(commandParser.IsValidColor("invalid_color"));
        }

        private class Graphics
        {
            internal static Graphics FromImage(Bitmap bitmap)
            {
                throw new NotImplementedException();
            }
        }
    }
}
