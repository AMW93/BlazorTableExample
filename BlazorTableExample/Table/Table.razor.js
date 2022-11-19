var dotnet;

export function init(tableElement, DotNetObjectReference) {
    enableColumnResizing(tableElement);
    dotnet = DotNetObjectReference;
    return {
        stop: () => { }
    };
}

function enableColumnResizing(tableElement) {
    $(tableElement).find('.col-width-draghandle').each(function (i, handle) {
        this.addEventListener('mousedown', handleMouseDown);
        if ('ontouchstart' in window) {
            handle.addEventListener('touchstart', handleMouseDown);
        }

        function handleMouseDown(evt) {
            evt.preventDefault();
            evt.stopPropagation();

            const th = $(this).closest('th')[0];
            const startPageX = evt.touches ? evt.touches[0].pageX : evt.pageX;
            const originalColumnWidth = th.offsetWidth;
            const rtlMultiplier = window.getComputedStyle(th, null).getPropertyValue('direction') === 'rtl' ? -1 : 1;
            let updatedColumnWidth = 0;

            function handleMouseMove(evt) {
                evt.stopPropagation();
                const newPageX = evt.touches ? evt.touches[0].pageX : evt.pageX;
                const nextWidth = originalColumnWidth + (newPageX - startPageX) * rtlMultiplier;
                if (Math.abs(nextWidth - updatedColumnWidth) > 0) {
                    updatedColumnWidth = nextWidth;
                    th.style.width = `${updatedColumnWidth}px`;
                }
            }

            function handleMouseUp() {
                document.body.removeEventListener('mousemove', handleMouseMove);
                document.body.removeEventListener('mouseup', handleMouseUp);
                document.body.removeEventListener('touchmove', handleMouseMove);
                document.body.removeEventListener('touchend', handleMouseUp);

                console.log('drag stopped');
                WidthCallback();
            }

            if (evt instanceof TouchEvent) {
                document.body.addEventListener('touchmove', handleMouseMove, { passive: true });
                document.body.addEventListener('touchend', handleMouseUp, { passive: true });
            } else {
                document.body.addEventListener('mousemove', handleMouseMove, { passive: true });
                document.body.addEventListener('mouseup', handleMouseUp, { passive: true });
            }
        }
    });
}

export async function WidthCallback() {
    await dotnet.invokeMethodAsync('WidthChanged');
}



////////////////////////////////////////////////////////////////////////////////

var row;
export function start() {
    row = event.target;
}
export function dragover() {
    var e = event;
    e.preventDefault();

    let children = Array.from(e.target.parentNode.parentNode.children);
    if (children.indexOf(e.target.parentNode) > children.indexOf(row))
        e.target.parentNode.after(row);
    else
        e.target.parentNode.before(row);
}