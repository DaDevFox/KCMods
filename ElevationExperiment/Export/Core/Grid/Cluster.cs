using System.Collections;
using System.Collections.Generic;

namespace Elevation
{

    public class Cluster : IEnumerable
    {
  
    public Dictionary<string, Node> ClustersGrid;
    
    public Dictionary<string, Node> ClustersUpperGrid;
    
    public Cluster( Dictionary<string, Node> clustersGrid){

        this.ClustersGrid = clustersGrid;
    }
    
    public void SetCluster(Dictionary<string, Node> clusterGrid, Dictionary<string, Node> clustesrGrid){

            this.ClustersGrid = clustesrGrid;
    }
    public void SetClustersUpper(Dictionary<string, Node> clustersUpperGrid){

            this.ClustersUpperGrid = clustersUpperGrid;
    }
    
    public Dictionary<string, Node> GetCluster(){
      
      return this.ClustersGrid;
    }
    
    public Dictionary<string, Node> GetClustersUpper(){
      
      return this.ClustersUpperGrid;
    }

        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
