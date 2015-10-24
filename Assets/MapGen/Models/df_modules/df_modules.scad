
module bound_box()
{
	translate([-1,-1,0])
		cube([2,2,3]);
}

module built_armor_stand()
{
	head_radius = 0.38;
	stand_radius = 0.12;
	stand_height = 1.5;
	base_radius = 0.6;
	base_height = 0.1;
	shoulder_width = 1.25;
	shoulder_height = 0.25;
	
	translate([0,0,.5])
		union()
		{
			cylinder(r = base_radius, h = base_height, $fn = 20);
			translate([0,0,base_height])
				cylinder(r = stand_radius, h = stand_height, $fn = 20);
			translate([0,0,stand_height + 0.2])
				sphere(r = head_radius, $fn = 20);
			translate([-shoulder_width/2,-.3/2,.9])
				cube([shoulder_width,.3,shoulder_height]);			
		}
}

module built_burial_receptacle()
{
	cof_width = 1;
	cof_height = 2.25;
	cof_depth = .6;
	
	translate([0,0,.5])
		union()
		{
			hull()
			{	
				translate([0,0,cof_height/2])
					cube([cof_width,cof_depth,cof_height], center = true);
				translate([0,0,.005 + 1.5])
					cube([cof_width + 0.8,cof_depth,.01], center = true);
			}
			
			translate([.55,-cof_depth/2,1.1])
				rotate([0,90,0])
					rotate_extrude(convexity = 10, $fn = 15)
						translate([.1,0,0])
							circle(r = .025, $fn = 15);
							
			translate([-.3,cof_depth/2,0])
				cylinder(r = 0.08, h = 2.4, $fn = 20);
			translate([.3,cof_depth/2,0])
				cylinder(r = 0.08, h = 2.5, $fn = 20);
		}
}

module built_door_x()
{
	door_height = 2.5;
	door_width = 2;
	door_depth = .5;
	
	translate([0,0,.5])
		union()
		{
			translate([0,0,door_height/2])
				difference()
				{
					cube([door_width,door_depth,door_height],center = true);
					for(x = [-door_width/4,door_width/4], y = [-door_depth/2,door_depth/2], z = [-door_height/4,door_height/4])
					{
						translate([x,y,z])
							hull()
							{
								cube([.5,.2,.5],center = true);
								cube([.75,.001,.75],center = true);
							}
					}
				}
			
			for(y = [-door_depth/2,door_depth/2])
			{
				translate([door_width/2 - .2,y,1.25])
					rotate([0,90,0])
						rotate_extrude(convexity = 10, $fn = 15)
							translate([.1,0,0])
								circle(r = .025, $fn = 15);		
			}
		}
}

module built_door_y()
{
	door_height = 2.5;
	door_width = 2;
	door_depth = .5;
	
	translate([0,0,.5])
	rotate([0,0,90])
		union()
		{
			translate([0,0,door_height/2])
				difference()
				{
					cube([door_width,door_depth,door_height],center = true);
					for(x = [-door_width/4,door_width/4], y = [-door_depth/2,door_depth/2], z = [-door_height/4,door_height/4])
					{
						translate([x,y,z])
							hull()
							{
								cube([.5,.2,.5],center = true);
								cube([.75,.001,.75],center = true);
							}
					}
				}
			
			for(y = [-door_depth/2,door_depth/2])
			{
				translate([door_width/2 - .2,y,1.25])
					rotate([0,90,0])
						rotate_extrude(convexity = 10, $fn = 15)
							translate([.1,0,0])
								circle(r = .025, $fn = 15);		
			}
		}
}

module built_floodgate_x()
{
	f_height = 2.5;
	f_width = 2;
	f_depth = .3;
	
	translate([0,0,.5 + f_height/2])
	union()
	{
		cube([f_width,f_depth,f_height],center = true);
		
		for(y = [f_depth/2,-f_depth/2], z = [0,2*f_height/5,-2*f_height/5])
		{
			translate([0,y,z])
				cube([f_width,.2,f_height/10],center = true);
		}
		
		for(x = [f_width/4,-f_width/4], y = [f_depth/2,-f_depth/2])
		{
			translate([x,y,0])
				cube([f_width/6,.2,f_height],center = true);
		}
		
		difference()
		{
			for(x = [f_width/4,-f_width/4])
			{
					translate([x,0,-f_height/2 - .15])
						rotate([0,90,0])
							rotate_extrude(convexity = 10, $fn = 20)
								translate([.75,0,0])
									circle(r = .125, $fn = 20);		
			}
			
			translate([-5,-5,-10 - f_height/2])
				cube([10,10,10]);
		}
	}
}

module built_floodgate_y()
{
	f_height = 2.5;
	f_width = 2;
	f_depth = .3;
	
	translate([0,0,.5 + f_height/2])
	rotate([0,0,90])
	union()
	{
		cube([f_width,f_depth,f_height],center = true);
		
		for(y = [f_depth/2,-f_depth/2], z = [0,2*f_height/5,-2*f_height/5])
		{
			translate([0,y,z])
				cube([f_width,.2,f_height/10],center = true);
		}
		
		for(x = [f_width/4,-f_width/4], y = [f_depth/2,-f_depth/2])
		{
			translate([x,y,0])
				cube([f_width/6,.2,f_height],center = true);
		}
		
		difference()
		{
			for(x = [f_width/4,-f_width/4])
			{
					translate([x,0,-f_height/2 - .15])
						rotate([0,90,0])
							rotate_extrude(convexity = 10, $fn = 20)
								translate([.75,0,0])
									circle(r = .125, $fn = 20);		
			}
			
			translate([-5,-5,-10 - f_height/2])
				cube([10,10,10]);
		}
	}
}

module built_floor_hatch()
{
	translate([0,0,.5 + .125/2])
		union()
		{
			cube([1.5,1.5,.125],center = true);
			
			translate([0,0,.125])
				cube([1.25,1.25,.125],center = true);

			translate([0,.4,.25 - .05])
				cube([1.25,.1,.05],center = true);

			translate([0,-.4,.25 - .05])
				cube([1.25,.1,.05],center = true);
				
			translate([.6,0,.18])
				rotate([0,12,0])
					rotate_extrude(convexity = 10, $fn = 20)
						translate([.1,0,0])
							circle(r = .02, $fn = 20);	
		}
}

module built_wall_grate_x()
{
	w_h = 2.5;
	w_w = 2;
	
	translate([0,0,.5])
		union()
		{
			for(x = [-w_w/4,w_w/4])
			{
				translate([x,0,0])
					cylinder(r = .15,h = w_h, $fn = 20);
			}
			for(z = [1*w_h/6,3*w_h/6,5*w_h/6])
			{
				translate([-w_w/2,0,z])
					rotate([0,90,0])
						cylinder(r = .15, h = w_w, $fn = 20);
			}
		}
}

module built_wall_grate_y()
{
	w_h = 2.5;
	w_w = 2;
	
	translate([0,0,.5])
	rotate([0,0,90])
		union()
		{
			for(x = [-w_w/4,w_w/4])
			{
				translate([x,0,0])
					cylinder(r = .15,h = w_h, $fn = 20);
			}
			for(z = [1*w_h/6,3*w_h/6,5*w_h/6])
			{
				translate([-w_w/2,0,z])
					rotate([0,90,0])
						cylinder(r = .15, h = w_w, $fn = 20);
			}
		}
}


%bound_box();
built_wall_grate_y();
