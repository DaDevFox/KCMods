using System.Collections;
using System.Collections.Generic;

namespace Elevation
{

    public class ClusterGrid : IEnumerable
    {

        public List<List<Cluster>> Clusters = new List<List<Cluster>>();

        public static int size;

        public ClusterGrid(int clusterDimentions, int height, int width){

            size = clusterDimentions * clusterDimentions;

            for(int i = 0; i < clusterDimentions; i++){
                
                List<Cluster> clusterRow = new List<Cluster>();
                for(int j = 0; j < clusterDimentions; j++){
                    
                    clusterRow.Add(new Cluster(new Dictionary<string, Elevation.PrebakedPathfinder.Node>((width / clusterDimentions) * (height / clusterDimentions)))); 
                }
                
                Clusters.Add(clusterRow);
            }
        }

        public List<List<Cluster>> GetClusters(){

            return this.Clusters;
        }

        public void SetClusters(List<List<Cluster>> clusters){

            this.Clusters = clusters;
        }

        public IEnumerator GetEnumerator()
        {
            return Clusters.GetEnumerator();
        }
    }
}
