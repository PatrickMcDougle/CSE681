// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------

namespace Parser.JSON
{
    public class Factory : IFactory
    {
        public IExpression CreateExpression(IToker toker)
        {
            return new Expression(toker);
        }

        public IToker CreateToker()
        {
            return new Toker();
        }
    }
}