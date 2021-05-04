using System.Collections;
using System.Collections.Generic;

namespace Elevation
{

    public class ClusterGrid : IEnumerable
    {

        public static List<List<Cluster>> Clusters = new List<List<Cluster>>();

        public static int size;

        public ClusterGrid(int clusterDimentions, int height, int width){

            size = clusterDimentions * clusterDimentions;

            for(int i = 0; i < size; i++){
                
                List<Cluster> clusterRow = new List<Cluster>();
                for(int j = 0; j < size; j++){
                    
                    clusterRow.Add(new Cluster(new Dictionary<string, Elevation.PrebakedPathfinder.Node>((width / clusterDimentions) * (height / clusterDimentions)))); 
                }
                
                Clusters.Add(clusterRow);
            }
        }


        public static List<List<Cluster>> GetClusters(){

            return ClusterGrid.Clusters;
        }

        public static void SetClusters(List<List<Cluster>> clusters){

            ClusterGrid.Clusters = clusters;
        }

        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
