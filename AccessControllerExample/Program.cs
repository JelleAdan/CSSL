﻿using CSSL.Examples.AccessController;
using CSSL.RL;

int responseLength = 1000;

RLLayer layer = new RLLayer();

RLController controller = new RLController(layer, responseLength);

controller.Run();

//RLLayer layer = new RLLayer();

//Stopwatch stopwatch = new Stopwatch();
//int reps = 100;
//double totalDuration = 0;

//Random rnd = new Random();
//ServerPool sp = layer.ac.ServerPool;

//for (int i = 0; i < reps; i++)
//{
//    stopwatch.Restart();

//    layer.Reset();
//    for (int customers = 0; customers < 6000; customers++)
//    {
//        if (sp.FreeServers > 0)
//        {
//            layer.Act(rnd.Next(2));
//        }
//        else
//        {
//            layer.Act(0);
//        }
//    }

//    totalDuration += stopwatch.Elapsed.TotalSeconds;
//}

//Console.WriteLine($"Average duration: {totalDuration / reps} seconds.");