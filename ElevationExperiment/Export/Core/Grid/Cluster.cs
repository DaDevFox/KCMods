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

  public class Cluster{
  
    public static Dictionary<string, Node> ClustersGrid;
    
    public static Dictionary<string, Node> ClustersUpperGrid;
    
    public static Cluster( Dictionary<string, Node> clustersGrid){
      
      this.ClustersGrid = clustersGrid;
    }
    
    public static SetCluster(Dictionary<string, Node> clusterGrid, Dictionary<string, Node> clustesrGrid){
       
      this.ClustersGrid = clustesrGrid;
    }
    public static SetClustersUpper(Dictionary<string, Node> clustersUpperGrid){
       
      this.ClustersUpperGrid = clustersUpperGrid;
    }
    
    public static Dictionary<string, Node> getCluster(){
      
      return this.ClustersGrid;
    }
    
    public static Dictionary<string, Node> getClustersUpper(){
      
      return this.ClustersUpperGrid;
    }
  }
}
