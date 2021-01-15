﻿using CSSL.Examples.AccessController;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace CSSL.RL
{
    public class RLController
    {
        public RLLayer RLLayer { get; set; }

        private MemoryMappedFile mmf { get; set; }

        public RLController(RLLayer RLLayer)
        {
            this.RLLayer = RLLayer;
            mmf = MemoryMappedFile.CreateNew("testmap", 10000, MemoryMappedFileAccess.ReadWrite);
        }

        public void Status()
        {
        }

        //public string Reset()
        //{
        //    try
        //    {
        //        return JsonSerializer.Serialize(RLLayer.Reset());
        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }
        //}

        //public string Act(int action)
        //{
        //    try
        //    {
        //        return JsonSerializer.Serialize(RLLayer.Act(action));
        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }
        //}
    }
}