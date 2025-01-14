using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace HotFix
{   
    public class CommandAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mresetProperty_0 = new CrossBindingMethodInfo("resetProperty");
        static CrossBindingMethodInfo<global::MyStringBuilder> mdebugInfo_2 = new CrossBindingMethodInfo<global::MyStringBuilder>("debugInfo");
        static CrossBindingMethodInfo<System.Boolean> msetDestroy_3 = new CrossBindingMethodInfo<System.Boolean>("setDestroy");
        static CrossBindingFunctionInfo<System.Boolean> misDestroy_4 = new CrossBindingFunctionInfo<System.Boolean>("isDestroy");
        static CrossBindingMethodInfo<System.Int64> msetAssignID_5 = new CrossBindingMethodInfo<System.Int64>("setAssignID");
        static CrossBindingFunctionInfo<System.Int64> mgetAssignID_6 = new CrossBindingFunctionInfo<System.Int64>("getAssignID");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(global::Command);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : global::Command, CrossBindingAdaptorType
        {
            protected CrossBindingMethodInfo mexecute_1 = new CrossBindingMethodInfo("execute");
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public override void resetProperty()
            {
                if (mresetProperty_0.CheckShouldInvokeBase(this.instance))
                    base.resetProperty();
                else
                    mresetProperty_0.Invoke(this.instance);
            }

            public override void execute()
            {
                if (mexecute_1.CheckShouldInvokeBase(this.instance))
                    base.execute();
                else
                    mexecute_1.Invoke(this.instance);
            }

            public override void debugInfo(global::MyStringBuilder builder)
            {
                if (mdebugInfo_2.CheckShouldInvokeBase(this.instance))
                    base.debugInfo(builder);
                else
                    mdebugInfo_2.Invoke(this.instance, builder);
            }

            public override void setDestroy(System.Boolean isDestroy)
            {
                if (msetDestroy_3.CheckShouldInvokeBase(this.instance))
                    base.setDestroy(isDestroy);
                else
                    msetDestroy_3.Invoke(this.instance, isDestroy);
            }

            public override System.Boolean isDestroy()
            {
                if (misDestroy_4.CheckShouldInvokeBase(this.instance))
                    return base.isDestroy();
                else
                    return misDestroy_4.Invoke(this.instance);
            }

            public override void setAssignID(System.Int64 assignID)
            {
                if (msetAssignID_5.CheckShouldInvokeBase(this.instance))
                    base.setAssignID(assignID);
                else
                    msetAssignID_5.Invoke(this.instance, assignID);
            }

            public override System.Int64 getAssignID()
            {
                if (mgetAssignID_6.CheckShouldInvokeBase(this.instance))
                    return base.getAssignID();
                else
                    return mgetAssignID_6.Invoke(this.instance);
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}
