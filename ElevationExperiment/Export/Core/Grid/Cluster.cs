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
    
    public static Cluster( Dictionary<string, Node> clusterGrid){
      
      this.ClusterGrid = clusterGrid;
    }
    
    public static SetCluster(Dictionary<string, Node> clusterGrid){
       
      this.ClusterGrid = clusterGrid;
    }
    
    public static Dictionary<string, Node> cluster(){
      
      return this.ClusterGrid;
    }
  }
}
