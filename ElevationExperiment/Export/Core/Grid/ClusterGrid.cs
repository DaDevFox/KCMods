
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using Newtonsoft.Json;

namespace Elevation
{

    public static class ClusterGrid{

        private List<List<Cluster>> Clusters = new List<List<Cluster>>();

        private int size;

        public ClusterGrid(int clusterDimentions, int height, int width){

            size = clusterDimentions * clusterDimentions;

            for(int i = 0; i < size; i++){

                for(int j = 0; j < size; j++){

                }
            }
        }

        public static List<List<Cluster> GetClusters(){

            return this.Clusters;
        }

        public static void SetClusters(List<List<Cluster> clusters){

            this.Clusters - clusters;
        }
    }
}
