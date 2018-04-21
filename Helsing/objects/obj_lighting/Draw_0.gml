var lx = obj_player.x;
var ly = obj_player.y;
var rad = 160; // the radius of the light
var tile_size = 32; // size of a tile
var tilemap = layer_tilemap_get_id("Tiles");


var startx = floor((lx-rad)/tile_size);
var endx = floor((lx+rad)/tile_size);
var starty = floor((ly-rad)/tile_size);
var endy = floor((ly+rad)/tile_size);

draw_set_color(c_yellow);
draw_rectangle(startx*tile_size,starty*tile_size, endx*tile_size,endy*tile_size,true);

/// @description init shadow casting
vertex_format_begin();
vertex_format_add_position();
vertex_format_add_color();
VertexFormat = vertex_format_end();

VBuffer = vertex_create_buffer();

vertex_begin(VBuffer, VertexFormat);
for(var yy=starty;yy<=endy;yy++)
{
    for(var xx=startx;xx<=endx;xx++)
    {
        var tile = tilemap_get(tilemap,xx,yy);
        if( tile!=0 )
        {
            // get corners of the 
            var px1 = xx*tile_size;     // top left
            var py1 = yy*tile_size;
            var px2 = px1+tile_size;    // bottom right
            var py2 = py1+tile_size;
			
			draw_set_color(c_black);
			draw_set_alpha(1 - (point_distance(lx, ly, px1, py1) / 32));
			draw_rectangle(px1, py1, px2, py2, false);
			
            ProjectShadow(VBuffer,  px1,py1, px2,py1, lx,ly );
            ProjectShadow(VBuffer,  px2,py1, px2,py2, lx,ly );
            ProjectShadow(VBuffer,  px2,py2, px1,py2, lx,ly );
            ProjectShadow(VBuffer,  px1,py2, px1,py1, lx,ly );                      
        }
    }
}
vertex_end(VBuffer);    
vertex_submit(VBuffer,pr_trianglelist,-1);
draw_set_alpha(1);