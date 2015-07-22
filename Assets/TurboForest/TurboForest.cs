using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class tfQuad // store each tree before generate batch mesh
{
	public static List<tfQuad> quads=new List<tfQuad>();
	
	public Vector3 pos;
	public float scale=1;
	
	public tfQuad(float x,float y, float z)
	{
		pos=new Vector3(x,y,z);
		quads.Add(this);
	}
	
}

public class TurboForest : MonoBehaviour 
{

	public Material treeMaterial;
	public float eachTreeBaseSize=1.5f;
	public float eachTreeSizeRandomize=0.2f;
	public float eachTreeShadingRandomize=0.5f;
	public int treesCount=100000;


	// each tree vertices
	Vector3 qv0=new Vector3(-1,-1,0);
	Vector3 qv1=new Vector3(1,-1,0);
	Vector3 qv2=new Vector3(1,1,0);
	Vector3 qv3=new Vector3(-1,1,0);

	// uv frame shift (8 frames per 8 rows in tree texture)
	const float frameSize=1.0f/8.0f;

	// each tree uvs
	Vector2 uv0=new Vector2(0,0);
	Vector2 uv1=new Vector2(frameSize,0);
	Vector2 uv2=new Vector2(frameSize,frameSize);
	Vector2 uv3=new Vector2(0,frameSize);
	
	void Start () 
	{

		if(this.GetComponent<Collider>()==null)
		{
			Debug.Log("ERR! object: "+this.name+" require collider to generate TurboForest");
			return;
		}

		qv0*=eachTreeBaseSize;
		qv1*=eachTreeBaseSize;
		qv2*=eachTreeBaseSize;
		qv3*=eachTreeBaseSize;

		GenerateTurboForest();
	}

    [ExecuteInEditMode]
    public void GenerateForest()
    {
        Start();
    }

	int Clamp(int v,int size)
	{
		if(v<0) return v+size;
		if(v>=size) return v-size;
		return v;
	}

	void GenerateTurboForest()
	{

		tfQuad.quads.Clear();
		
		Vector3 tpos=new Vector3(0,100000,0); // raycast from (to check if tree on mesh)
		float castDistance=Mathf.Abs(tpos.y*2);

		Ray ray=new Ray();
		ray.direction=-Vector3.up;

		RaycastHit info;

		// forest area from collider
		Vector3 cmin=GetComponent<Collider>().bounds.min;
		Vector3 cmax=GetComponent<Collider>().bounds.max;

		int total=treesCount;

		int isize=100; // grid place trees step

		float step=(cmax.x-cmin.x)/isize;
		float step2=step*2;

		// calc trees height on edges and randomize heights
		float[,] height=new float[isize,isize];
		float[,] blur=new float[isize,isize];

		for(int i=0;i<isize;i++)
		for(int j=0;j<isize;j++)
		{
			height[i,j]=.5f;

			tpos.x=cmin.x+i*step;
			tpos.z=cmin.z+j*step;
			ray.origin=tpos;
			if(this.GetComponent<Collider>().Raycast(ray,out info,castDistance))
			{
				height[i,j]=1;
				if(Random.value<.3f) height[i,j]=Random.value*.5f;
			}
		}

		for(int t=0;t<4;t++)
		{
			for(int i=0;i<isize;i++)
			for(int j=0;j<isize;j++)
			{
				blur[i,j]=0;
				for(int ii=-1;ii<2;ii++)
				for(int jj=-1;jj<2;jj++)
				{
					blur[i,j]+=height[Clamp(i+ii,isize),Clamp(j+jj,isize)];
				}
				blur[i,j]/=9;
			}
			for(int i=0;i<isize;i++)
			for(int j=0;j<isize;j++)
			{
				height[i,j]=blur[i,j];
			}
		}
		// heights calculated here

		while(total>0) // till all trees places
		{
			// fill one mesh
			for(int i=0;i<isize;i++)
			for(int j=0;j<isize;j++)
			{

				// tree position
				tpos.x=cmin.x+i*step-step+Random.value*step2;
				tpos.z=cmin.z+j*step-step+Random.value*step2;

				ray.origin=tpos;

				if(this.GetComponent<Collider>().Raycast(ray,out info,castDistance)) // if tree on mesh
				{

					tfQuad q=new tfQuad(tpos.x,info.point.y,tpos.z);
					q.scale=height[i,j]*(1.0f-eachTreeSizeRandomize+Random.value*eachTreeSizeRandomize*2);
					total--;

					if(tfQuad.quads.Count==10666) // max quads per mesh (42 664 indices)
					{
						BuildMesh();
						tfQuad.quads.Clear(); // clear quads list for next mesh
						System.GC.Collect();
					}




				}
			}



		}

		if(tfQuad.quads.Count>0)
		{
			BuildMesh();
			tfQuad.quads.Clear(); // clear quads list for next mesh
			System.GC.Collect();
		}

	}

	void BuildMesh()
	{

		if(tfQuad.quads.Count==0) return;

		Vector3[] verts=new Vector3[tfQuad.quads.Count*4]; // all trees (in mesh) vertices
		Vector2[] uvs=new Vector2[tfQuad.quads.Count*4]; // trees uvs
		Vector2[] uvs2=new Vector2[tfQuad.quads.Count*4]; // uvs2 store tre type, one of 4 trees texture shift
		Vector3[] normals=new Vector3[tfQuad.quads.Count*4]; // normals store each tree position in mesh (billboards no need normals ;)
		
		int[] indices=new int[tfQuad.quads.Count*4]; // quads in mesh

		// need to calculate min and max bounds manualy, for correct AABB Unity calc
		Vector3 min=tfQuad.quads[0].pos;
		Vector3 max=min;
		
		Vector2 typeShift=new Vector2(0,0); // shift current on texture (tree type, one of 4)

		for(int i=0;i<tfQuad.quads.Count;i++) // fill arrays
		{
			
			tfQuad q=tfQuad.quads[i];

			// AABB calc
			min.x=Mathf.Min(min.x,q.pos.x);
			min.y=Mathf.Min(min.y,q.pos.y);
			min.z=Mathf.Min(min.z,q.pos.z);
			
			max.x=Mathf.Max(max.x,q.pos.x);
			max.y=Mathf.Max(max.y,q.pos.y);
			max.z=Mathf.Max(max.z,q.pos.z);

			int ii=i*4;

			// vertices of tree
			verts[ii]=qv0*q.scale;
			verts[ii+1]=qv1*q.scale;
			verts[ii+2]=qv2*q.scale;
			verts[ii+3]=qv3*q.scale;
			
			// randomize shading
			float tint=1.0f-eachTreeShadingRandomize+(Random.value*eachTreeShadingRandomize*2);
			verts[ii].z=tint;
			verts[ii+1].z=tint;
			verts[ii+2].z=tint;
			verts[ii+3].z=tint;
			
			// base tree uvs (first frame of this tree)
			uvs[ii]=uv0;
			uvs[ii+1]=uv1;
			uvs[ii+2]=uv2;
			uvs[ii+3]=uv3;

			indices[ii]=ii;
			indices[ii+1]=ii+1;
			indices[ii+2]=ii+2;
			indices[ii+3]=ii+3;

			// push tree up from groung on it height
			q.pos.y+=-qv0.y*q.scale;

			// write tree position to it normals (for shader)
			normals[ii]=q.pos;
			normals[ii+1]=q.pos;
			normals[ii+2]=q.pos;
			normals[ii+3]=q.pos;

			// calc and write tree type
			typeShift.x=0;
			typeShift.y=0;
			
			int type=0;
			
			float r=Random.value;
			if(r>.25f) type=1;
			if(r>.5f) type=2;
			if(r>.75f) type=3;
			
			if(type==1) typeShift.x=.5f;
			else if(type==2) typeShift.y=.5f;
			else if(type==3) {typeShift.x=.5f;typeShift.y=.5f;}
			
			uvs2[ii]=typeShift;
			uvs2[ii+1]=typeShift;
			uvs2[ii+2]=typeShift;
			uvs2[ii+3]=typeShift;
			
		}

		// creating mesh
		GameObject trees=new GameObject();
		MeshFilter mf=trees.AddComponent<MeshFilter>();
		
		mf.sharedMesh=new Mesh();
		
		mf.sharedMesh.vertices=verts;
		
		mf.sharedMesh.normals=normals;
		mf.sharedMesh.uv=uvs;
		mf.sharedMesh.uv2=uvs2;
		mf.sharedMesh.SetIndices(indices,MeshTopology.Quads,0);

		mf.sharedMesh.bounds = new Bounds(center: (min + max) / 2, size: max - min); // alexzzzz fix

		MeshRenderer mr=trees.AddComponent<MeshRenderer>();
		mr.sharedMaterial=treeMaterial;

	}

}
