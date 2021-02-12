using System;
using System.Collections.Generic;
using CSLox.Lexer;
using CSLox.Parsing;

namespace CSLox.Interpreting
{
    public class LoxClass : ICallable
    {
        private readonly string name;
        private readonly Dictionary<string, LoxFunction> methods;

        public string Name => name;

        public int Arity 
        {
            get
            {
                var initializer = FindMethod(this.name);
                if (initializer == null) return 0;
                return initializer.Arity;
            }
        }

        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            this.name = name;
            this.methods = methods;
        }

        public LoxFunction FindMethod(string name)
        {
            if (methods.ContainsKey(name))
            {
                return methods[name];
            }

            return null;
        }

        public override string ToString()
        {
            return name;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var instance = new LoxInstance(this);
            var initializer = FindMethod(this.name);
            if (initializer != null)
            {
                initializer.Bind(instance).Call(interpreter, arguments);
            }

            return instance;
        }
    }
}