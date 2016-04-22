namespace AOPIOC.Wcf
{
    using System;

    public class Lifecycle
    {
        public const int CONTAINER_CONTROLLED = 1;
        public const int DEFAULT = 0;
        public const int PER_THREAD = 2;
        public const int WCF_INSTANCE = 3;
    }
}

