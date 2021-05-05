using System.Collections;
using System.Collections.Generic;

namespace Elevation
{

    public class ClusterGrid : IEnumerable
    {

        public List<List<Cluster>> Clusters;

        public static int size;

        public ClusterGrid(int clusterDimentions, int height, int width){

            Clusters = new List<List<Cluster>>(width / clusterDimentions);

            size = (height / clusterDimentions) * (width / clusterDimentions);

            for(int i = 0; i < height / clusterDimentions; i++){
                
                List<Cluster> clusterRow = new List<Cluster>(height / clusterDimentions);
                for(int j = 0; j < width / clusterDimentions; j++){
                    
                    clusterRow.Add(new Cluster(new Dictionary<string, Elevation.PrebakedPathfinder.Node>(clusterDimentions * clusterDimentions), new Dictionary<string, Elevation.PrebakedPathfinder.Node>(clusterDimentions * clusterDimentions))); 
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
