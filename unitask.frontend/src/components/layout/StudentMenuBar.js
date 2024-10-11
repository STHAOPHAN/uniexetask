import React from 'react';
import { Menu, LayoutDashboard, Users, FolderKanban, MessageSquare } from 'lucide-react';

const StudentMenuBar = () => {
    return (
        <nav className="bg-[#281942] top-16 left-0 w-full fixed z-20">
          <div className="container mx-auto px-4">
            <div className="flex items-center justify-between h-16">
              <div className="flex items-center">
                <div className="flex-shrink-0 text-white">
                  <Menu size={24} />
                </div>
                <div className="hidden md:block">
                  <div className="ml-10 flex items-baseline space-x-4">
                    <a href="/topics" className="text-white hover:bg-[#602d84] hover:text-white px-3 py-2 rounded-md text-sm font-medium">
                      <LayoutDashboard className="inline-block mr-1" size={16} /> Topics
                    </a>
                    <a href="/groups" className="text-white hover:bg-[#602d84] hover:text-white px-3 py-2 rounded-md text-sm font-medium">
                      <Users className="inline-block mr-1" size={16} /> Groups
                    </a>
                    <a href="/projects" className="text-white hover:bg-[#602d84] hover:text-white px-3 py-2 rounded-md text-sm font-medium">
                      <FolderKanban className="inline-block mr-1" size={16} /> Projects
                    </a>
                    <a href="#" className="text-white hover:bg-[#602d84] hover:text-white px-3 py-2 rounded-md text-sm font-medium">
                      <MessageSquare className="inline-block mr-1" size={16} /> Trò chuyện
                    </a>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </nav>
      );
};

export default StudentMenuBar;