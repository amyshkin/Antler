﻿namespace SmartElk.Antler.Core.Abstractions.Configuration
{
    public class BasicConfiguration: IBasicConfiguration
    {
        public IContainer Container { get; set; }

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}
