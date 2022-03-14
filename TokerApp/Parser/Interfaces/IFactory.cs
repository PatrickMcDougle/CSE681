// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public interface IFactory
    {
        IExpression CreateExpression(IToker toker);

        IToker CreateToker();
    }
}