using Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.BotCommands;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Controllers
{
    class TestNumbersController : IController
    {
        public int ValueOfTestInt { get; private set; }
        public uint ValueOfTestUInt { get; private set; }
        public long ValueOfTestLong { get; private set; }
        public ulong ValueOfTestULong { get; private set; }
        public double ValueOfTestDouble { get; private set; }
        public decimal ValueOfTestDecimal { get; private set; }

        public void TestNumbers(NumbersCommand command, Contexts contexts)
        {
            this.ValueOfTestInt = command.TestInt;
            this.ValueOfTestUInt = command.TestUInt;
            this.ValueOfTestLong = command.TestLong;
            this.ValueOfTestULong = command.TestULong;
            this.ValueOfTestDouble = command.TestDouble;
            this.ValueOfTestDecimal = command.TestDecimal;
        }

        public void TestInt(IntCommand command, Contexts contexts)
        {
            ValueOfTestInt = command.TestInt;
        }

        public void TestUInt(UIntCommand command, Contexts contexts)
        {
            ValueOfTestUInt = command.TestUInt;
        }

        public void TestLong(LongCommand command, Contexts contexts)
        {
            ValueOfTestLong = command.TestLong;
        }

        public void TestULong(ULongCommand command, Contexts contexts)
        {
            ValueOfTestULong = command.TestULong;
        }

        public void TestDouble(DoubleCommand command, Contexts contexts)
        {
            ValueOfTestDouble = command.TestDouble;
        }

        public void TestDecimal(DecimalCommand command, Contexts contexts)
        {
            ValueOfTestDecimal = command.TestDecimal;
        }
    }
}
