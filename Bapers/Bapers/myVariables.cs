using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bapers
{
    public class myVariables
    {
        //cust
        public static string currfname = "";
        public static string currlname = "";
        public static string currnum = "";
        public static string currID = "";
        public static string currvalue = "";

        //user
        public static string role = "";
        
        //stack to store page history
        public static Stack<string> myStack = new Stack<string>();
       
    }
}
