using System.Collections;
using System.Collections.Generic;

namespace Elevation
{

    public class Cluster
    {
  
        public Dictionary<string, Elevation.PrebakedPathfinder.Node> ClustersGrid = new Dictionary<string, Elevation.PrebakedPathfinder.Node>();
    
        public Dictionary<string, Elevation.PrebakedPathfinder.Node> ClustersUpperGrid = new Dictionary<string, Elevation.PrebakedPathfinder.Node>();

        public Dictionary<string, GridRoute> Routes;

        public Cluster( Dictionary<string, Elevation.PrebakedPathfinder.Node> clustersGrid, Dictionary<string, Elevation.PrebakedPathfinder.Node> clustersUpperGrid)
        {

            this.ClustersGrid = clustersGrid;
            this.ClustersUpperGrid = clustersUpperGrid;
        }
    
        public void SetCluster(Dictionary<string, Elevation.PrebakedPathfinder.Node> clusterGrid, Dictionary<string, Elevation.PrebakedPathfinder.Node> clustesrGrid){

            this.ClustersGrid = clustesrGrid;
        }
        public void SetClustersUpper(Dictionary<string, Elevation.PrebakedPathfinder.Node> clustersUpperGrid){

            this.ClustersUpperGrid = clustersUpperGrid;
        }
    
        public Dictionary<string, Elevation.PrebakedPathfinder.Node> GetCluster(){
      
            return this.ClustersGrid;
        }
    
        public Dictionary<string, Elevation.PrebakedPathfinder.Node> GetClustersUpper(){
      
            return this.ClustersUpperGrid;
        }
    }
}
