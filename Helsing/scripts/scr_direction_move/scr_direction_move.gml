/// argument0 = the object moving
/// argument1 = the direction to move

if (argument0.obj_direction == argument1)
{
	if (argument1 == DIR_UP)
	{
		argument0.y = y - GRID_SIZE;
	}
	else if (argument1 == DIR_DOWN)
	{
		argument0.y = y + GRID_SIZE;
	}
		else if (argument1 == DIR_LEFT)
	{
		argument0.x = x - GRID_SIZE;
	}
		else if (argument1 == DIR_RIGHT)
	{
		argument0.x = x + GRID_SIZE;
	}
}

argument0.obj_direction = argument1;
argument0.image_angle = argument1;