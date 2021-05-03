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
  
    public static Dictionary<string, Node> ClusterGrid;
    
    public static Dictionary<string, Node> ClusterUpperGrid;
    
    public static Cluster( Dictionary<string, Node> clusterGrid){
      
      this.ClusterGrid = clusterGrid;
    }
    
    public static SetCluster(Dictionary<string, Node> clusterGrid, Dictionary<string, Node> clusterGrid){
       
      this.ClusterGrid = clusterGrid;
    }
    public static SetUpperCluster(Dictionary<string, Node> clusterUpperGrid){
       
      this.ClusterUpperGrid = clusterUpperGrid;
    }
    
    public static Dictionary<string, Node> getCluster(){
      
      return this.ClusterGrid;
    }
    
    public static Dictionary<string, Node> getUpperCluster(){
      
      return this.ClusterUpperGrid;
    }
  }
}
