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
  
    public static Grid ClusterGrid;
    
    public static Cluster( Grid clusterGrid){
      
      this.ClusterGrid = clusterGrid;
    }
    
    public static SetCluster(Grid clusterGrid){
       
      this.ClusterGrid = clusterGrid;
    }
    
    public static Grid cluster(){
      
      return this.ClusterGrid;
    }
  }
}
