using System.Collections;
using System.Collections.Generic;

namespace Elevation
{

    public class Cluster : IEnumerable
    {
  
    public Dictionary<string, Elevation.PrebakedPathfinder.Node> ClustersGrid;
    
    public Dictionary<string, Elevation.PrebakedPathfinder.Node> ClustersUpperGrid;
    
    public Cluster( Dictionary<string, Elevation.PrebakedPathfinder.Node> clustersGrid){

        this.ClustersGrid = clustersGrid;
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

        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
