using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services.cameraInfo
{
  public class CameraInfoProvider
  {
    public List<Camera> Cameras { get; set; } = new List<Camera>();
  }
}