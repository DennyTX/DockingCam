using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLDD_camera.Utils
{
    public class ShaderInfo
    {
        List<string> validShaders = null;

        public bool IsValid(string shader)
        {
            if (validShaders == null || validShaders.Count == 0)
                return true;
            return validShaders.Contains(shader);
        }

        int cnt = 0;
        public string FirstShader
        {
            get
            {
                if (validShaders == null || validShaders.Count == 0)
                    return "";
                cnt = 0;
                return validShaders.First();
            }
        }

        public string NextShader
        {
            get
            {
                if (validShaders == null || validShaders.Count == 0 || cnt >= validShaders.Count - 1)
                    return "";
                cnt++;
                return validShaders[cnt];
            }
        }

        public string PrevShader
        {
            get
            {
                if (validShaders == null || validShaders.Count == 0 || cnt <= 0)
                    return "";
                cnt--;
                return validShaders[cnt];
            }
        }

        public ShaderInfo(string shaders)
        {
            validShaders = shaders.Split(',').ToList();
        }
    }

}
